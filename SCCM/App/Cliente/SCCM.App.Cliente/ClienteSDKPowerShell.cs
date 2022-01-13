using SCCM.App.Dominio;
using SCCM.App.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.App.Cliente
{
    public class ClienteSDKPowerShell
    {
        private static PSRunspace _runspace = null;
        public ClienteSDKPowerShell()
        {
            _runspace = new PSRunspace();
        }
        public void Conectar()
        {
            _runspace.DefinirRunspace();
        }
        public void Desconectar()
        {
            _runspace.RemoverRunspace();
        }
        public ClientSDKInstalResult ExecutarScriptApp(string chave, string scriptApp)
        {
            Conectar();

            ClientSDKInstalResult sdkInstalResult = new ClientSDKInstalResult();
            sdkInstalResult = _runspace.Executar(chave, scriptApp, sdkInstalResult);

            Desconectar();

            return sdkInstalResult;
        }
    }
}
