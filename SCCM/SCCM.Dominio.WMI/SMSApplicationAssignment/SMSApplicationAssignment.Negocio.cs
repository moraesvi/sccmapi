using SCCM;
using SCCM.Dominio.Comum;
using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.WMI
{
    public partial class SMSApplicationAssignment
    {
        /// <summary>
        /// Realiza a implantação de instalação de um aplicativo
        /// </summary>
        /// <returns>retorna true[IMPLANTADO]</returns>
        public bool Implantar(SMSApplication application, string idColecao, DeployOfferTypeID acaoDeploy)
        {
            SMSCollection collection = new SMSCollection();
            collection = collection.Obter(idColecao);

            if (collection == null)
            {
                throw new InvalidOperationException("Ocorreu um erro ao realizar o Deploy", new Exception("A coleção inserida não existe no SCCM."));
            }

            SMSAppParaSMSAppAssignment(application, collection, DeployType.Install, acaoDeploy);

            string scriptModelPS = PSInstance(this);
            scriptModelPS = string.Concat(scriptModelPS, "\n", string.Format("$SMSApplicationAssignment.OfferTypeID = {0}", (int)acaoDeploy));
            scriptModelPS = string.Concat(scriptModelPS, "\n", "$SMSApplicationAssignment.SuppressReboot = 0");

            bool inserido = PSRequisicao.PUT(scriptModelPS, this);

            return inserido;
        }
        /// <summary>
        /// Realiza a implantação de remoção de um aplicativo
        /// </summary>
        /// <returns>retorna true[IMPLANTADO]</returns>
        public bool Remover(SMSApplication application, string idColecao, DeployOfferTypeID acaoDeploy)
        {
            SMSCollection collection = new SMSCollection();
            collection = collection.Obter(idColecao);

            if (collection == null)
            {
                throw new InvalidOperationException("Ocorreu um erro ao realizar o Deploy", new Exception("A coleção inserida não existe no SCCM."));
            }

            SMSAppParaSMSAppAssignment(application, collection, DeployType.Uninstall, acaoDeploy);

            string scriptModelPS = PSInstance(this);
            scriptModelPS = string.Concat(scriptModelPS, "\n", string.Format("$SMSApplicationAssignment.OfferTypeID = {0}", (int)acaoDeploy));
            scriptModelPS = string.Concat(scriptModelPS, "\n", "$SMSApplicationAssignment.SuppressReboot = 0");

            bool inserido = PSRequisicao.PUT(scriptModelPS, this);

            return inserido;
        }
        /// <summary>
        /// Realiza a remoção da implantacao de um aplicativo
        /// </summary>
        /// <returns>retorna objeto de auditoria</returns>
        public PSObjetoRemovidoAudit RemoverImplatacao(SMSDeploymentInfo implantacao)
        {
            PSObjetoRemovidoAudit objPSDeletadoAudit = RemoverDeploy(implantacao.DeploymentID);

            return objPSDeletadoAudit;
        }

        #region Metodos Privados
        private PSObjetoRemovidoAudit RemoverDeploy(string deploymentID)
        {
            SMSDeploymentInfo smsDeploy = new SMSDeploymentInfo();
            smsDeploy = smsDeploy.Obter(deploymentID);

            if (smsDeploy == null)
            {
                throw new InvalidOperationException("O deploy não existe no SCCM.");
            }
            if (!smsDeploy.DeploymentName.Contains("[SCCM_API]"))
            {
                throw new InvalidOperationException("Operação inválida de remoção de deploy", new Exception("O deploy é restrito."));
            }

            PSQuery.AddFiltro("AssignmentUniqueID", deploymentID);
            PSObjetoRemovidoAudit objPSDeletadoAudit = PSRequisicao.DeleletarQueryResult(PSQuery.ToString());

            return objPSDeletadoAudit;
        }
        private void SMSAppParaSMSAppAssignment(SMSApplication application, SMSCollection collection, DeployType tipoDeploy, DeployOfferTypeID acaoDeploy)
        {
            switch (acaoDeploy)
            {
                case DeployOfferTypeID.Avaiable:
                    DefinirDeployAvaiable(application, collection, tipoDeploy);
                    break;
                case DeployOfferTypeID.Required:
                    DefinirDeployRequired(application, collection, tipoDeploy);
                    break;
                default:
                    throw new InvalidOperationException("Ocorreu um erro ao realizar o Deploy", new Exception(string.Concat("Não foi possível definir o tipo de deploy: ", application.LocalizedDisplayName)));
            }

        }
        private void DefinirDeployRequired(SMSApplication application, SMSCollection collection, DeployType tipoDeploy)
        {
            /*
                SCCM - Deploy Required - Não necessário ação do usuário.
             */
            string dataAgora = DateTime.Now.ToDateTimePowerShell();

            string deploymentDesc = (tipoDeploy == DeployType.Install) ? "Instalar" :
                                                  (tipoDeploy == DeployType.Uninstall) ? "Desinstalar"
                                                  : string.Empty;

            this.ApplicationName = application.LocalizedDisplayName;
            this.AssignedCI_UniqueID = application.CI_UniqueID;
            this.AssignedCIs = new int[] { Convert.ToInt32(application.CI_ID) };
            this.AssignmentName = string.Concat(application.LocalizedDisplayName, "[SCCM_API]_Deployment", deploymentDesc);
            this.DesiredConfigType = (int)tipoDeploy;

            //Dados de coleção

            this.CollectionName = collection.Name;
            this.DisableMomAlerts = true;
            this.AssignmentDescription = string.Concat("[SCCM_API] - REQUIRED - Criado pela API do SCCM ", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            this.EnforcementDeadline = dataAgora;
            this.NotifyUser = false;
            this.OfferFlags = 1;

            this.DesiredConfigType = (int)DeployDesiredConfigType.Required;
            this.OverrideServiceWindows = true;
            this.RebootOutsideOfServiceWindows = false;
            this.RequireApproval = false;
            this.StartTime = dataAgora;

            //Coleção

            this.TargetCollectionID = collection.CollectionID;
            this.UseGMTTimes = false;
            this.UserUIExperience = true;
            this.WoLEnabled = false;
            this.LocaleID = 1033;
        }

        private void DefinirDeployAvaiable(SMSApplication application, SMSCollection collection, DeployType tipoDeploy)
        {
            /*
                SCCM - Deploy Avaiable - Necessário ação do usuário na instalação do aplicativo.
             */
            string dataAgora = DateTime.Now.ToDateTimePowerShell();

            string deploymentDesc = (tipoDeploy == DeployType.Install) ? "Instalar" :
                                                  (tipoDeploy == DeployType.Uninstall) ? "Desinstalar"
                                                  : string.Empty;

            this.ApplicationName = application.LocalizedDisplayName;
            this.AssignmentName = string.Concat(application.LocalizedDisplayName, "[SCCM_API]_Deployment", deploymentDesc);
            this.AssignedCIs = new int[] { Convert.ToInt32(application.CI_ID) };
            this.DesiredConfigType = (int)tipoDeploy;

            //Dados de coleção

            this.CollectionName = collection.Name;
            this.CreationTime = dataAgora;
            this.StartTime = dataAgora;
            this.SourceSite = SCCMHelper.SMSSite;
            this.AssignmentDescription = string.Concat("[SCCM_API]- AVAIABLE - Criado pela API do SCCM ", dataAgora);
            this.NotifyUser = true;

            this.DesiredConfigType = 1;
            this.OverrideServiceWindows = true;
            this.RebootOutsideOfServiceWindows = false;
            this.RequireApproval = false;

            //Coleção

            this.TargetCollectionID = collection.CollectionID;
            this.WoLEnabled = false;
            this.RebootOutsideOfServiceWindows = false;
            this.OverrideServiceWindows = false;
            this.UseGMTTimes = true;
            this.LocaleID = 1043;
        }

        #endregion
    }
}
