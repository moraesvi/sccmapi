using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel
{
    public class PSComando
    {
        private static Runspace _remoteRunspace;
        private static TraceSource _tsPSCode;
        private static TraceSwitch _debugLevel;

        public static void DefinirRunspace(Runspace RemoteRunspace, TraceSource PSCode)
        {
            _remoteRunspace = RemoteRunspace;
            _tsPSCode = PSCode;
            _debugLevel = new TraceSwitch("DebugLevel", "DebugLevel from ConfigFile", "Verbose");

            //Cache = null;//new MemoryCache(RemoteRunspace.ConnectionInfo.ComputerName, new System.Collections.Specialized.NameValueCollection());
        }

        public static List<WMIPropriedade> WMIListarParaNet(string caminhoClasseWMI, bool newtonsoftJsonSerialization)
        {
            try
            {
                List<WMIPropriedade> lstResult = new List<WMIPropriedade>();
                string sPSCode = caminhoClasseWMI;

                foreach (PSObject obj in WSMan.ExecutarPSScript(sPSCode, _remoteRunspace))
                {
                    try
                    {
                        string propNet = string.Empty;

                        propNet = Parse.PropNet(obj.ToString());

                        if (!string.IsNullOrEmpty(propNet))
                        {
                            if (newtonsoftJsonSerialization)
                            {
                                propNet = string.Concat("[JsonProperty]", "\n", propNet);
                            }

                            lstResult.Add(new WMIPropriedade(propNet));
                        }

                        /*possui = Parse.ContemGetSet(obj);

                        if (possui)
                        {
                            propNet = Parse.PropNet(obj.ToString());

                            if(!string.IsNullOrEmpty(propNet))
                                lstResult.Add(new WMIPropriedade(propNet));
                        }

                        possui = Parse.ContemMembros(obj);*/

                        /*if (possui)
                        {
                            PSMemberInfo[] membros = obj.Members.ToArray();

                            membros.ToList().ForEach(valor =>
                            {
                                string propriedade = Parse.FormatarPropriedade(valor.TypeNameOfValue, valor.Name);
                                propNet = Parse.PropNet(propriedade);

                                if (!string.IsNullOrEmpty(propNet))
                                    lstResult.Add(new WMIPropriedade(propNet));
                            });
                        }*/
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLineIf(_debugLevel.TraceError, ex.Message);
                    }
                }

                //Trace do comando PowerShell
                _tsPSCode.TraceInformation(sPSCode);

                return lstResult;
            }
            catch
            {
                return new List<WMIPropriedade>();
            }
        }
    }
}
