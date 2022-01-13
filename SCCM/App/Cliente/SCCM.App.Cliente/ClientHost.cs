using SCCM.App.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.App.Cliente
{
    public class ClientHost
    {
        private static string PS_QUERY = "Get-WmiObject -Class Win32_OperatingSystem | ForEach-Object -MemberName Caption";
        private static PSRunspace _runspace = null;
        public ClientHost()
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
        public string SistemaOperacional()
        {
            try
            {
                Conectar();

                string hostname = string.Empty;
                hostname = _runspace.Executar(PS_QUERY).ToString();

                return hostname;
            }
            catch
            {
                Desconectar();
                return null;
            }
            finally
            {
                Desconectar();
            }
        }
    }
}
