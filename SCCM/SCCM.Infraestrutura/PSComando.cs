using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Infraestrutura
{
    public class PSComando
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
        public static ICollection<T> ExecutarColecao<T>(string query, T objeto) where T : class
        {
            ICollection<T> colecaoObjeto = WSMan.ExecutarPSScriptColecao(query, objeto, _remoteRunspace);

            return colecaoObjeto;
        }
        public static ICollection<T> ExecutarColecao<T>(string script, string comando, List<Chave> param, List<ValorChave> paramValor, T objeto, bool arquivo) where T : class
        {
            ICollection<T> colecaoObjeto = WSMan.ExecutarPSScriptColecao(script, comando, param, paramValor, arquivo, objeto, _remoteRunspace);

            return colecaoObjeto;
        }
        public static ICollection<T> ExecutarColecao<T>(string query, string nomeParam, object param, T objeto) where T : class
        {
            ICollection<T> colecaoObjeto = WSMan.ExecutarPSScriptColecao(query, nomeParam, param, objeto, _remoteRunspace);

            return colecaoObjeto;
        }
        public static bool ExecutarResult(string script)
        {
            bool result = WSMan.ExecutarResultPSScript(script, _remoteRunspace);

            return result;
        }
        public static T Executar<T>(string query, T objeto) where T : class
        {
            T objetoResult = WSMan.ExecutarPSScript(query, objeto, _remoteRunspace);

            return objetoResult;
        }
        public static T Executar<T>(string query, string nomeParam, object param, T objeto) where T : class
        {
            T objetoResult = WSMan.ExecutarPSScript(query, nomeParam, param, objeto, _remoteRunspace);

            return objetoResult;
        }
        public static T Executar<T>(string script, string comando, List<Chave> param, List<ValorChave> paramValor, T objeto, bool arquivo) where T : class
        {
            T objetoResult = WSMan.ExecutarPSScript(script, comando, param, paramValor, arquivo, objeto, _remoteRunspace);

            return objetoResult;
        }
        public static PSObjetoRemovidoAudit DeletarQueryResult(string queryCompleto)
        {
            PSObjetoRemovidoAudit psDeletadoAudit = WSMan.DeletarQueryResult(queryCompleto, _remoteRunspace);

            return psDeletadoAudit;
        }
        public static bool ValidarFuncaoExiste(string nomeFuncao)
        {
            bool psFuncaoExiste = WSMan.ValidarFuncaoExiste(nomeFuncao, _remoteRunspace);

            return psFuncaoExiste;
        }
        public static PSFuncaoExisteAudit ValidarFuncaoExisteResult(string nomeFuncao)
        {
            PSFuncaoExisteAudit psFuncaoExiste = WSMan.ValidarFuncaoExisteResult(nomeFuncao, _remoteRunspace);

            return psFuncaoExiste;
        }
        public static bool PUT<T>(string scriptModelPS, T objeto) where T : class
        {
            bool result = WSMan.ExecutarPSScriptPUT(scriptModelPS, objeto, _remoteRunspace);

            return result;
        }
        public static TResult PUTResult<T, TResult>(string scriptModelPS, T objeto, TResult objetoResult) where T : class
                                                                                                         where TResult : class
        {
            TResult result = WSMan.ExecutarPSScriptPUTResult(scriptModelPS, objeto, objetoResult, _remoteRunspace);

            return result;
        }
        public static ICollection<PSObject> Executar(string query, string[] param)
        {
            ICollection<PSObject> colecaoObjeto = WSMan.ExecutarPSScript(query, param, _remoteRunspace);

            return colecaoObjeto;
        }
        public static ICollection<string> Executar(string query)
        {
            ICollection<string> lstResult = new List<string>();
            ICollection<PSObject> colecaoPSObject = WSMan.ExecutarPSScript(query, _remoteRunspace);

            colecaoPSObject.ToList().ForEach(PSObject =>
            {
                lstResult.Add(PSObject.ToString());
            });

            return lstResult;
        }
    }
}
