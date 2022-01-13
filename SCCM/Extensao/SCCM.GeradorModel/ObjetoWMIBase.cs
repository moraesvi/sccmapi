using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel
{
    public class WMIObjetoBase : IObjetoWMI
    {
        private string _query;
        private bool _newtonsoftJsonSerialization;
        public WMIObjetoBase(string query, bool newtonsoftJsonSerialization)
        {
            _query = query;
            _newtonsoftJsonSerialization = newtonsoftJsonSerialization;
        }
        public WMIModelResult Obter(Runspace remoteRunspace, TraceSource PSCode)
        {
            try
            {
                PSComando.DefinirRunspace(remoteRunspace, PSCode);

                WMIModelResult modelResult = new WMIModelResult();

                List<WMIPropriedade> lstProp = PSComando.WMIListarParaNet(_query, _newtonsoftJsonSerialization);

                modelResult.NomeClasse = this.GetType().Name;
                modelResult.Prop = lstProp;

                return modelResult;
            }
            catch
            {
                throw;
            }
        }
    }
}
