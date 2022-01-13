using SCCM.Servico.Contratos;
using SCCM.Servico.Dominio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Servico.PowerShell.Infra
{
    public class PSRunspace
    {
        private static Runspace _remoteRunspace;
        private static TraceSource _tsPSCode;
        private static TraceSwitch _debugLevel;

        public static void DefinirRunspace(Runspace remoteRunspace, TraceSource PSCode)
        {
            _remoteRunspace = remoteRunspace;
            _tsPSCode = PSCode;
            _debugLevel = new TraceSwitch("DebugLevel", "DebugLevel from ConfigFile", "Verbose");
        }
        public static bool ExecutarResult(string script)
        {
            try
            {
                bool result = PSExecucao.ExecutarResultPSScript(script, _remoteRunspace);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public static KeyValuePair<bool, string> ExecutarResultXML(string script, string chave, string dispositivo)
        {
            try
            {
                KeyValuePair<bool, string> result = PSExecucao.ExecutarResultXMLPSScript(script, dispositivo, chave, _remoteRunspace);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public static KeyValuePair<bool, string> ExecutarResult(string script, string dispositivo, string chave, IPowerShellResult powerShellResult)
        {
            try
            {
                KeyValuePair<bool, string> result = PSExecucao.ExecutarResultPSScript(script, dispositivo, chave, powerShellResult, _remoteRunspace);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public static PSArquivoResult[] ExecutarBuscaArquivo(string caminho)
        {
            try
            {
                PSArquivoResult[] arrayArquivos = PSExecucao.ExecutarBuscaArquivoPSScript(caminho, _remoteRunspace)
                                                            .ToArray();

                return arrayArquivos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
}
