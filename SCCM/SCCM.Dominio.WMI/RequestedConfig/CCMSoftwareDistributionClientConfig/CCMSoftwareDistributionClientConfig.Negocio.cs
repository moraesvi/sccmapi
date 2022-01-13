using SCCM.Dominio.Comum;
using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.WMI.RequestedConfig
{
    public partial class CCMSoftwareDistributionClientConfig
    {
        public CCMSoftwareDistributionClientConfig[] ObterLocalPolitica(string localPolitica)
        {
            CCMSoftwareDistributionClientConfig[] result = PSRequisicao.ExecutarColecao(PSQuery, "PolicySource", localPolitica, this);

            return result;
        }
        public PSObjetoRemovidoAudit DeletarPoliticas(string localPolitica)
        {
            PSQuery.AddFiltro("PolicySource", localPolitica);
            PSObjetoRemovidoAudit objPSDeletadoAudit = PSRequisicao.DeleletarQueryResult(PSQuery.ToString());

            return objPSDeletadoAudit;
        }
        public bool Desabilitar()
        {
            AcutualConfig.CCMSoftwareDistributionClientConfig politicasClient = new AcutualConfig.CCMSoftwareDistributionClientConfig();

            bool habilitado = politicasClient.Habilitado();

            if (habilitado)
            {
                DefinirForcar();
                string scriptModelPS = PSInstance(this);

                bool realizado = PSRequisicao.PUT(scriptModelPS, this);

                return realizado;
            }

            return false;
        }
        private void DefinirForcar()
        {
            this.ComponentName = "SmsSoftwareDistribution";
            this.Enabled = false;
            this.LockSettings = true;
            this.PolicySource = "local";
            this.PolicyVersion = "1.0";
            this.SiteSettingsKey = 1;
        }
    }
}
