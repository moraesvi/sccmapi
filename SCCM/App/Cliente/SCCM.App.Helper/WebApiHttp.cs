using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.App.Helper
{
    public class WebApiHttp
    {
        public static string HttpJsonRequisicao(int totalTentativas, string urlApi)
        {
            string json = string.Empty;

            if (string.IsNullOrWhiteSpace(urlApi))
            {
                throw new InvalidOperationException("Operação Inválida", new Exception("Não foi possível buscar ou definir o endereço da API"));
            }

            for (int indice = 0; indice < totalTentativas; indice++)
            {
                try
                {
                    json = HelperComum.WebApiHttp.Requisicao(urlApi);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(string.Concat("API - Ocorreu um erro na requisição de busca URL:", urlApi), new Exception(ex.InnerException != null ? ex.InnerException.Message : null));
                }

                if (!string.IsNullOrWhiteSpace(json))
                    break;
            }

            return json;
        }
        public static string HttpJsonRequisicaoPost(int totalTentativas, string urlApi, object data)
        {
            string json = string.Empty;

            if (string.IsNullOrWhiteSpace(urlApi))
            {
                throw new InvalidOperationException("Operação Inválida", new Exception("Não foi possível buscar ou definir o endereço da API"));
            }

            for (int indice = 0; indice < totalTentativas; indice++)
            {
                try
                {
                    json = HelperComum.WebApiHttp.RequisicaoPost(urlApi, data);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(string.Concat("API - Ocorreu um erro na requisição de busca URL:", urlApi), new Exception(ex.InnerException != null ? ex.InnerException.Message : null));
                }

                if (!string.IsNullOrWhiteSpace(json))
                    break;
            }

            return json;
        }
    }
}
