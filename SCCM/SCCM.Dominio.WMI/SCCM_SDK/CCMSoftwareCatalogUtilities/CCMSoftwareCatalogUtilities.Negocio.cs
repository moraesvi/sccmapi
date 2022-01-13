using SCCM.Dominio.Comum;
using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.WMI
{
    public partial class CCMSoftwareCatalogUtilities
    {
        public IWMIResult DeviceIDResult()
        {
            string deviceId = DeviceID();

            object result = new { DeviceId = deviceId, CatalogUtilities = this };

            return WMIResult.Resultado(result);
        }
        public IWMIResult PortalUrlResult()
        {
            string portalURL = PortalUrl();

            object result = new { PortalUrl = portalURL };

            return WMIResult.Resultado(result);
        }
        public string PortalUrl()
        {
            string psScript = string.Concat("$clientsdk = ", INSTANCE, ";");
            psScript = string.Concat(psScript, "\n$clientsdk.GetPortalUrlValue();");

            string portalUrl = PSRequisicao.Executar(psScript);

            if (string.IsNullOrEmpty(portalUrl))
            {
                throw new InvalidOperationException("Operação inválida", new Exception("Não foi possível definir a URL do Catálago de Aplicativos do SCCM"));
            }

            return portalUrl;
        }
        public string DeviceID()
        {
            string deviceId = string.Empty;
            string psScript = string.Concat("$clientsdk = ", INSTANCE, ";");
            psScript = string.Concat(psScript, "\n$clientsdk.GetDeviceID();");

            CCMSoftwareCatalogUtilities objetoResult = PSRequisicao.Executar(psScript, this);

            if (string.IsNullOrEmpty(objetoResult.ClientId) || string.IsNullOrEmpty(objetoResult.SignedClientId))
            {
                throw new InvalidOperationException("Operação inválida", new Exception("Não foi possível definir o DeviceId do SCCM"));
            }
            else
            {
                deviceId = string.Concat(objetoResult.ClientId, ",", objetoResult.SignedClientId);
            }

            return deviceId;
        }
    }
}
