using SCCM.Dominio.Comum;
using SCCM.Infraestrutura;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.PowerShell
{
    public class PSRequisicao
    {
        private const string FILRO_WHERE = "WHERE {0} = {1}";
        private const string FILRO_WHERE_STRING = "WHERE {0} = '{1}'";
        private const string FILRO_AND = "AND {0} = {1}";
        private const string FILRO_AND_STRING = "AND {0} = '{1}'";

        public static void DefinirRunspace(Runspace remoteRunspace, TraceSource PSCode)
        {
            Infraestrutura.PSComando.DefinirRunspace(remoteRunspace, PSCode);
        }

        public static bool ExecutarResult(string script)
        {
            bool objetoResult = Infraestrutura.PSComando.ExecutarResult(script);

            return objetoResult;
        }
        public static T Executar<T>(string script, T objeto) where T : class
        {
            T result = Infraestrutura.PSComando.Executar<T>(script, objeto);

            return result;
        }
        public static T Executar<T>(PSQuery query, string nomeParam, object param, T objeto) where T : class
        {
            string queryFiltro = PSQueryFiltroFormat(query, nomeParam, param);

            T objetoResult = Infraestrutura.PSComando.Executar<T>(queryFiltro, nomeParam, param, objeto);

            return objetoResult;
        }
        public static T[] ExecutarColecao<T>(PSQuery query, string nomeParam, object param, T objeto) where T : class
        {
            string queryFiltro = PSQueryFiltroFormat(query, nomeParam, param);

            T[] arrayResult = Infraestrutura.PSComando.ExecutarColecao<T>(queryFiltro, nomeParam, param, objeto).ToArray();

            return arrayResult;
        }
        public static T[] ExecutarColecao<T>(string query, T objeto) where T : class
        {
            T[] arrayResult = Infraestrutura.PSComando.ExecutarColecao<T>(query, objeto).ToArray();

            return arrayResult;
        }
        public static T Executar<T>(Dominio.Comum.PSComando PSComando, List<ValorChave> paramValor, T objeto) where T : class
        {
            T result = Infraestrutura.PSComando.Executar<T>(PSComando.Script, PSComando.Comando, PSComando.Param, paramValor, objeto, PSComando.Arquivo);

            return result;
        }
        public static T[] ExecutarColecao<T>(Dominio.Comum.PSComando PSComando, List<ValorChave> paramValor, T objeto) where T : class
        {
            T[] arrayResult = Infraestrutura.PSComando.ExecutarColecao<T>(PSComando.Script, PSComando.Comando, PSComando.Param, paramValor, objeto, PSComando.Arquivo).ToArray();

            return arrayResult;
        }
        public static bool ValidarFuncaoExiste(string nomeFuncao)
        {
            bool result = Infraestrutura.PSComando.ValidarFuncaoExiste(nomeFuncao);

            return result;
        }
        public static PSFuncaoExisteAudit ValidarFuncaoExisteResult(string nomeFuncao)
        {
            PSFuncaoExisteAudit psFuncaoExiste = Infraestrutura.PSComando.ValidarFuncaoExisteResult(nomeFuncao);

            return psFuncaoExiste;
        }
        public static PSObjetoRemovidoAudit DeleletarQueryResult(string queryCompleto)
        {
            PSObjetoRemovidoAudit psDeletadoAudit = Infraestrutura.PSComando.DeletarQueryResult(queryCompleto);

            return psDeletadoAudit;
        }
        public static bool PUT<T>(string scriptModelPS, T objeto) where T : class
        {
            bool result = Infraestrutura.PSComando.PUT(scriptModelPS, objeto);

            return result;
        }
        public static TResult PUTResult<T, TResult>(string scriptModelPS, T objeto, TResult objetoResult) where T : class
                                                                                                          where TResult : class
        {
            TResult result = Infraestrutura.PSComando.PUTResult(scriptModelPS, objeto, objetoResult);

            return result;
        }
        public static string Executar(string scriptQuery)
        {
            string result = Infraestrutura.PSComando.Executar(scriptQuery).SingleOrDefault();

            return result;
        }
        public static string[] ExecutarColecao(string scriptQuery)
        {
            ICollection<string> colecaoPSObject = Infraestrutura.PSComando.Executar(scriptQuery);

            return colecaoPSObject.ToArray();
        }
        private static string PSQueryFiltroFormat(PSQuery psQuery, string nomeParam, object param)
        {
            string queryFiltro = string.Empty;

            if (param.GetType() == typeof(string) || param.GetType().IsClass)
            {
                if (psQuery.Query.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase) >= 0)
                    queryFiltro = string.Format(string.Concat(psQuery.Query, " ", FILRO_AND_STRING), nomeParam, param);
                else
                    queryFiltro = string.Format(string.Concat(psQuery.Query, " ", FILRO_WHERE_STRING), nomeParam, param);
            }
            else
            {
                //queryFiltro = string.Format(string.Concat(psQuery.Query, " ", FILRO_WHERE), nomeParam, param);
                if (psQuery.Query.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase) >= 0)
                    queryFiltro = string.Format(string.Concat(psQuery.Query, " ", FILRO_AND), nomeParam, param);
                else
                    queryFiltro = string.Format(string.Concat(psQuery.Query, " ", FILRO_WHERE), nomeParam, param);
            }

            return new PSQuery(psQuery.Namespace, queryFiltro).ToString();
        }
    }
}
