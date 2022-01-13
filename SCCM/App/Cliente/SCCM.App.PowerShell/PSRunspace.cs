using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.App.PowerShell
{
    public class PSRunspace : IDisposable
    {
        private static Runspace _remoteRunspace;
        private static TraceSource _tsPSCode;
        private static TraceSwitch _debugLevel;

        public void DefinirRunspace()
        {
            if (Conectado())
                RemoverRunspace();

            _remoteRunspace = RunspaceFactory.CreateRunspace();
            _remoteRunspace.Open();

            _tsPSCode = new TraceSource("PSCode");

            RunspaceInvoke rsInvoker = new RunspaceInvoke(_remoteRunspace);

            //Tentativa de uso inrestrito, caso o usuário seja administrador, não necessário.
            try
            {
                rsInvoker.Invoke("Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy Unrestricted");
            }
            catch { }
        }
        public void DefinirRunspace(Runspace remoteRunspace, TraceSource PSCode)
        {
            _remoteRunspace = remoteRunspace;
            _tsPSCode = PSCode;
            _debugLevel = new TraceSwitch("DebugLevel", "DebugLevel from ConfigFile", "Verbose");

            RunspaceInvoke rsInvoker = new RunspaceInvoke(_remoteRunspace);
            rsInvoker.Invoke("Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy Unrestricted");
        }
        public void RemoverRunspace()
        {
            _remoteRunspace.Close();
            _remoteRunspace.Dispose();
        }
        public T Executar<T>(string chave, string script, T objeto) where T : class
        {
            PSExecucao.DefinirChave(chave);

            T result = PSExecucao.ExecutarPSScriptAsync(script, objeto, _remoteRunspace);

            return result;
        }
        public object Executar(string script)
        {
            object result = PSExecucao.ExecutarPSScript(script, _remoteRunspace);

            return result;
        }
        public void Dispose()
        {
            try
            {
                if (Conectado())
                {
                    _remoteRunspace.Close();
                }
            }
            catch { }

            if (_remoteRunspace != null)
                _remoteRunspace.Dispose();

            GC.SuppressFinalize(this);
        }

        #region Metodos Privados
        private bool Conectado()
        {
            if (_remoteRunspace != null)
            {
                if (_remoteRunspace.RunspaceStateInfo.State == RunspaceState.Opened)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
