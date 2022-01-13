using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel
{
    static class WSMan
    {
        internal static void ConexaoRunspace(WSManConnectionInfo connectionInfo, ref Runspace remoteRunspace)
        {
            remoteRunspace = RunspaceFactory.CreateRunspace(connectionInfo);
            remoteRunspace.Open();
        }

        internal static string ExecutarPSScriptAsString(string scriptText, Runspace remoteRunspace)
        {
            StringBuilder stringBuilder = new StringBuilder();

            ICollection<PSObject> results = ExecutarPSScript(scriptText, remoteRunspace);

            foreach (PSObject obj in results)
            {
                try
                {
                    if (obj != null)
                    {
                        stringBuilder.AppendLine(obj.ToString());
                    }
                }
                catch { }
            }


            return stringBuilder.ToString();
        }

        /// <summary>
        /// Executa um script PowerShell
        /// </summary>
        /// <param name="scriptText"></param>
        /// <param name="remoteRunspace"></param>
        /// <returns></returns>
        internal static ICollection<PSObject> ExecutarPSScript(string script, Runspace remoteRunspace)
        {

            try
            {
                using (PowerShell powershell = PowerShell.Create())
                {
                    powershell.Runspace = remoteRunspace;
                    powershell.AddScript(script);

                    ICollection<PSObject> PSresults = powershell.Invoke();
                    List<PSObject> loRes = PSresults.Where(t => t != null).ToList();
                    ICollection<PSObject> results = new List<PSObject>();

                    foreach (PSObject po in loRes)
                    {
                        if (po != null)
                            results.Add(po);
                    }

                    if (loRes.Count == 0)
                    {
                        ICollection<ErrorRecord> errors = powershell.Streams.Error.ReadAll();
                        foreach (ErrorRecord er in errors)
                        {
                            PSObject pErr = new PSObject(er);
                            results.Add(pErr);
                        }
                    }

                    return results;
                }
            }
            catch
            { }

            return null;
        }
    }
}
