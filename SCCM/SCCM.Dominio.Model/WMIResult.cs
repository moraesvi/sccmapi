using HelperComum;
using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Model
{
    internal class WMIResult
    {
        private static readonly string[] _arrayDominioValido = { DOMINIO_PRBBR, DOMINIO_BSBR, DOMINIO_ADTeste };

        private const string DOMINIO_ADTeste = "ADTeste";
        private const string DOMINIO_PRBBR = "PRBBR";
        private const string DOMINIO_BSBR = "BSCH";

        public static IWMIResult Resultado<TResult>(TResult resultado, string msgDescricao = null) 
        {
            string nomeDominio = string.Empty;

            #region Definir Dominio
            try
            {
                Win32ComputerSystemDomain objDominio = new Win32ComputerSystemDomain();
                nomeDominio = objDominio.Obter()
                                        .Domain;

                string[] dominios = nomeDominio.Split('.');

                nomeDominio = _arrayDominioValido.Where(valor => dominios.Contains(valor, new CompararOrdinal()))
                                                 .FirstOrDefault();

                nomeDominio = nomeDominio.ToUpper() == DOMINIO_BSBR ? "BSBR" : nomeDominio;

                if (string.IsNullOrEmpty(nomeDominio))
                {
                    throw new InvalidOperationException("Dominio SCCM inválido.", new InvalidOperationException("O domínio do SCCM não é válido."));
                }
            }
            catch (Exception ex)
            {
                string msg = string.Concat("Ocorreu um erro na busca do domínio - ", resultado.GetType().Name);

                throw new Exception(msg, ex.InnerException());
            }
            #endregion

            try
            {
                string msgResult = string.Empty;

                if (resultado.GetType().IsArray)
                {
                    msgResult = !string.IsNullOrWhiteSpace(msgDescricao) ? msgDescricao : string.Concat("Script executado - ", resultado.GetType().GetElementType().Name);
                }
                else
                {
                    msgResult = !string.IsNullOrWhiteSpace(msgDescricao) ? msgDescricao : string.Concat("Script executado - ", resultado.GetType().Name);
                }

                IWMIResult WMIResult = WMIResultFactory.Criar(msgResult, nomeDominio, resultado);

                return WMIResult;
            }
            catch (Exception ex)
            {
                string msg = string.Concat("Ocorreu um erro na definição do objeto de resultado - ", resultado.GetType().Name);

                throw new Exception(msg, ex.InnerException());
            }
        }
    }
}
