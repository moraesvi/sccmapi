using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.WMI.SCCM_SDK
{
    public partial class CCMApplication
    {
        public CCMApplication Instalar(string appId)
        {
            CCMApplication app = this.Obter(appId);

            string psAppInstalacao = PSInstalarComando(app);

            try
            {
                bool procInstalacao = PSRequisicao.ExecutarResult(psAppInstalacao);
            }
            catch
            {
                throw new Exception("Ocorreu um erro na instalação do aplicativo", new Exception("Erro na execução do comando powershell de instalação de aplicativo"));
            }

            app.ApplicabilityState = StatusInstalacao(appId);

            return app;
        }
        public string StatusInstalacao(string appId)
        {
            CCMApplication app = this.Obter(appId);

            return app.ApplicabilityState;
        }
        public bool VerificarReiniciarPC()
        {
            try
            {
                string psReiniciarPCSoft = "$reiniciarPCSoft = ([wmiclass]'root/ccm/clientSDK:CCM_ClientUtilities').DetermineIfRebootPending().RebootPending";
                string psReiniciarPC = "$reiniciarPC = ([wmiclass]'root/ccm/clientSDK:CCM_ClientUtilities').DetermineIfRebootPending().IsHardRebootPending";

                bool reiniciarPCSoft = PSRequisicao.ExecutarResult(psReiniciarPCSoft);
                bool reiniciarPC = PSRequisicao.ExecutarResult(psReiniciarPC);

                return (reiniciarPCSoft || reiniciarPC);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro na verificação de reinicialização do computador", ex.InnerException);
            }
        }
        private string PSInstalarComando(CCMApplication app)
        {
            string scriptInstall = string.Concat("([wmiclass]'ROOT\\ccm\\ClientSdk:CCM_Application').Install(", app.Id, app.Revision, string.Concat("$", app.IsMachineTarget.ToString()), 0, "Normal", "$False", ")");

            return scriptInstall;
        }
    }
}
