using SCCM.Dominio.Comum;
using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.WMI.AcutualConfig
{
    public partial class CCMSoftwareDistributionClientConfig : SMSBase<CCMSoftwareDistributionClientConfig>
    {
        public bool Habilitado()
        {
            try
            {
                PSQuery PSQueryHab = new PSQuery(SCCMHelper.CCMActualConfigNamespace, "SELECT Enabled FROM CCM_SoftwareDistributionClientConfig");
                CCMSoftwareDistributionClientConfig politicasClient = PSRequisicao.Executar(PSQueryHab, "SiteSettingsKey", 1, this);

                return politicasClient.Enabled;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
}
