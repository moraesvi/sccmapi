using SCCM.Dominio.Comum;
using SCCM.Dominio.Model;
using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCCM.Dominio.WMI
{
    public partial class SMSApplication
    {
        /// <summary>
        /// Realiza a implantação de um aplicativo
        /// </summary>
        /// <returns>retorna true[IMPLANTADO]</returns>
        public bool ImplantarAplicativo(string ci_UniqueID, string idColecao, DeployOfferTypeID tipoDeploy)
        {
            SMSCollection col = new SMSCollection();
            bool realizado = false;

            SMSCollection colecao = col.Obter(idColecao);

            if (colecao == null)
                throw new InvalidOperationException("Transação inválida, coleção possui mais de 4 membros.");

            if (!string.IsNullOrWhiteSpace(ci_UniqueID) && colecao != null)
            {
                realizado = Implantar(ci_UniqueID, idColecao, tipoDeploy);
            }

            return realizado;
        }
        /// <summary>
        /// Realiza a implantação de instalação de um aplicativo forçando o cliente
        /// </summary>
        /// <returns>retorna true[IMPLANTADO]</returns>
        public bool ImplantarAplicativoForcar(string ci_UniqueID, string dominio, string usuario, string chaveCookie, DeployOfferTypeID tipoDeploy)
        {
            SMSCollection col = new SMSCollection();
            bool implantado = false;

            if (!string.IsNullOrWhiteSpace(ci_UniqueID))
            {
                implantado = ImplantarForcar(ci_UniqueID, dominio, usuario, chaveCookie, tipoDeploy);
            }

            return implantado;
        }
        /// <summary>
        /// Realiza a implatação de remoção de um aplicativo
        /// </summary>
        /// <returns>retorna true[IMPLANTADO]</returns>
        public bool RemoverAplicativo(string ci_UniqueID, string idColecao, DeployOfferTypeID tipoDeploy)
        {
            SMSCollection col = new SMSCollection();
            bool realizado = false;

            SMSCollection colecao = col.Obter(idColecao);

            if (colecao != null && colecao.MemberCount > 4)
                throw new InvalidOperationException("Transação inválida, coleção possui mais de 4 membros.");

            if (!string.IsNullOrWhiteSpace(ci_UniqueID) && colecao != null)
            {
                realizado = Implantar(ci_UniqueID, idColecao, tipoDeploy);
            }

            return realizado;
        }
        /// <summary>
        /// Realiza a implatação de remoção de um aplicativo forçando o cliente
        /// </summary>
        /// <returns>retorna true[IMPLANTADO]</returns>
        public bool RemoverAplicativoForcar(string ci_UniqueID, string dominio, string usuario, string chaveCookie, DeployOfferTypeID tipoDeploy)
        {
            bool implantado = false;
            SMSCollection col = new SMSCollection();

            if (!string.IsNullOrWhiteSpace(ci_UniqueID))
            {
                implantado = RemoverForcar(ci_UniqueID, dominio, usuario, chaveCookie, tipoDeploy);
            }

            return implantado;
        }
        /// <summary>
        /// Realiza a remoção da implantacao de um aplicativo
        /// </summary>
        /// <returns>retorna objeto de auditoria</returns>
        public PSObjetoRemovidoAudit RemoverImplatacao(SMSDeploymentInfo implantacao)
        {
            SMSApplicationAssignment smsAppDeploy = new SMSApplicationAssignment();

            PSObjetoRemovidoAudit objPSDeletadoAudit = smsAppDeploy.RemoverImplatacao(implantacao);

            return objPSDeletadoAudit;
        }
        /// <summary>
        /// Realiza a remoção da implantacao de um aplicativo
        /// </summary>
        /// <returns>retorna objeto de auditoria</returns>
        public PSObjetoRemovidoAudit RemoverImplatacao(string deploymentID)
        {
            SMSApplicationAssignment smsAppDeploy = new SMSApplicationAssignment();
            SMSDeploymentInfo smsDeploymentInfo = new SMSDeploymentInfo();

            smsDeploymentInfo.DeploymentID = deploymentID;

            PSObjetoRemovidoAudit objPSDeletadoAudit = smsAppDeploy.RemoverImplatacao(smsDeploymentInfo);

            return objPSDeletadoAudit;
        }
        public bool ExisteModel(string modelName)
        {
            PSQuery.AddFiltro("ModelName", modelName);

            SMSApplication obj = PSRequisicao.Executar(PSQuery.ToString(), this);

            if (obj.CI_UniqueID == null)
                return false;

            return true;
        }

        #region Metodos Privados
        private bool Implantar(string ci_UniqueID, string idColecao, DeployOfferTypeID tipoDeploy)
        {
            if (!string.IsNullOrWhiteSpace(ci_UniqueID))
            {
                SMSApplication smsApp = this.Obter(ci_UniqueID);
                SMSApplicationAssignment smsAppDeploy = new SMSApplicationAssignment();

                bool realizado = smsAppDeploy.Implantar(smsApp, idColecao, tipoDeploy);

                return realizado;
            }

            return false;
        }
        private bool ImplantarForcar(string ci_UniqueID, string dominio, string usuario, string chaveCookie, DeployOfferTypeID tipoDeploy)
        {
            if (string.IsNullOrWhiteSpace(ci_UniqueID))
            {
                return false;
            }

            SMSApplication smsApp = this.Obter(ci_UniqueID);
            SMSApplicationAssignment smsAppDeploy = new SMSApplicationAssignment();

            #region Colecao Unico Usuario

            string usuarioImpl = string.Concat(usuario, "_APPInstalar");
            SMSCollection colecao = new SMSCollection();
            SMSCollection[] arrayColecao = colecao.ObterUsuario(usuarioImpl);

            if (arrayColecao != null && arrayColecao.Count() > 0)
            {
                arrayColecao.ToList().ForEach(col =>
                {
                    colecao.Remover(col);
                });
            }

            PSColecaoAudit colecaoAudit = colecao.AdicionarUsuarioMembro(dominio, usuario, usuarioImpl);

            if (colecaoAudit == null)
                throw new InvalidOperationException("Operação inválida", new Exception("Ocorreu um erro na criação da coleção única do usuário"));

            string idColecaoUsuario = colecaoAudit.CollectionId();

            #endregion

            bool implantado = smsAppDeploy.Implantar(smsApp, idColecaoUsuario, tipoDeploy);

            if (implantado)
            {
                ForcarImplatacao(ci_UniqueID, dominio, usuario, chaveCookie);
            }

            return implantado;
        }
        private bool Remover(string ci_UniqueID, string idColecao, DeployOfferTypeID tipoDeploy)
        {
            if (!string.IsNullOrWhiteSpace(ci_UniqueID))
            {
                SMSApplication smsApp = this.Obter(ci_UniqueID);
                SMSApplicationAssignment smsAppDeploy = new SMSApplicationAssignment();

                bool realizado = smsAppDeploy.Remover(smsApp, idColecao, tipoDeploy);

                return realizado;
            }

            return false;
        }
        private bool RemoverForcar(string ci_UniqueID, string dominio, string usuario, string chaveCookie, DeployOfferTypeID tipoDeploy)
        {
            if (string.IsNullOrWhiteSpace(ci_UniqueID))
            {
                return false;
            }

            SMSApplication smsApp = this.Obter(ci_UniqueID);
            SMSApplicationAssignment smsAppDeploy = new SMSApplicationAssignment();

            #region Colecao Unico Usuario

            string usuarioImpl = string.Concat(usuario, "_APPRemover");
            SMSCollection colecao = new SMSCollection();
            SMSCollection[] arrayColecao = colecao.ObterUsuario(string.Concat(usuarioImpl));

            if (arrayColecao != null && arrayColecao.Count() > 0)
            {
                colecao.Remover(arrayColecao.SingleOrDefault());
            }

            PSColecaoAudit colecaoAudit = colecao.AdicionarUsuarioMembro(dominio, usuario, usuarioImpl);

            if (colecaoAudit == null)
                throw new InvalidOperationException("Operação inválida", new Exception("Ocorreu um erro na criação da coleção única do usuário"));

            string idColecaoUsuario = colecaoAudit.CollectionId();

            #endregion

            bool implantado = smsAppDeploy.Remover(smsApp, idColecaoUsuario, tipoDeploy);

            if (implantado)
            {
                ForcarImplatacao(ci_UniqueID, dominio, usuario, chaveCookie);
            }

            return implantado;
        }
        public void ForcarImplatacao(string ci_UniqueID, string dominio, string usuario, string chaveCookie)
        {
            Task<bool> taskforce = Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(4000);

                SMSClient smsClient = new SMSClient();
                SMSCollection colecao = new SMSCollection();

                List<ChaveValor> lstExecucao = new List<ChaveValor>();

                List<PSComandoParam> comandosParam = new List<PSComandoParam>()
                {
                        new PSComandoParam(1, "Disp"),
                };

                    //string chave = GerarChaveForceArquivo(ci_UniqueID, usuario);

                    bool realizado = smsClient.ExecutarComandoValidarLogado(comandosParam.ToArray(), chaveCookie, chaveCookie, dominio, usuario, "Trigger-Force", "Trigger-Force");

                System.Threading.Thread.Sleep(4000);

                colecao = colecao.ObterUsuario(usuario).SingleOrDefault();

                    //colecao.Remover(colecao.CollectionID);

                    return true;
            });
        }
        private List<ChaveValor> RealizarComandoForce(string ci_UniqueID, string dominio, string usuario, string chaveCookie, SMSClient smsClient)
        {
            List<ChaveValor> lstExecucao = new List<ChaveValor>();

            List<PSComandoParam> comandosParam = new List<PSComandoParam>()
            {
                new PSComandoParam(1, "HOST"),
            };

            string chave = GerarChaveForceArquivo(ci_UniqueID, usuario);

            bool realizado = smsClient.ExecutarComandoValidarLogado(comandosParam.ToArray(), chave, chaveCookie, dominio, usuario, "Trigger-Force", "Trigger-Force");

            lstExecucao.Add(new ChaveValor() { Chave = "", Valor = realizado });

            return lstExecucao;
        }
        public string GerarChaveForceArquivo(string ci_UniqueID, string usuario)
        {
            string scopeId = "ScopeId_";
            string applicationId = "Application_";

            if (string.IsNullOrWhiteSpace(ci_UniqueID))
            {
                throw new InvalidOperationException("FORCE Operação inválida", new Exception("O CI_ID não foi inserido."));
            }

            try
            {
                int indice = (ci_UniqueID.IndexOf(applicationId) - applicationId.Length);

                ci_UniqueID = ci_UniqueID.Replace(scopeId, "").Substring(0, indice);
                ci_UniqueID = string.Concat(usuario, "_", ci_UniqueID);
            }
            catch
            {
                throw new InvalidOperationException("FORCE Operação inválida", new Exception("O CI_ID inserido não é válido."));
            }

            return ci_UniqueID;
        }

        #endregion
    }
}
