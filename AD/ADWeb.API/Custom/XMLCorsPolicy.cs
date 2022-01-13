using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http.Cors;
using System.Xml.Linq;

namespace ADWeb.API
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class XMLCorsPolicy : Attribute, ICorsPolicyProvider
    {
        private const string ARQUIVO_XML_DEFAULT = "Cors.xml";
        private const string CHAVE_CACHE = "XML";
        private const string XML_NODE_DEFAULT = "URL";
        private CorsPolicy _policy;

        public XMLCorsPolicy()
        {
            _policy = new CorsPolicy
            {
                AllowAnyMethod = true,
                AllowAnyHeader = true
            };

            string[] arrayURI = ReadXMLURIs();

            foreach (var uriOrigem in arrayURI)
            {
                _policy.Origins.Add(uriOrigem);
            }
        }
        public Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_policy);
        }

        #region Metodos Privados
        private string[] ReadXMLURIs()
        {
            try
            {
                List<string> lstURI = new List<string>();

                string currentDir = System.AppDomain.CurrentDomain.BaseDirectory;
                XDocument xml = null;

                try
                {
                    string script = System.IO.Path.Combine(currentDir, ARQUIVO_XML_DEFAULT);
                    xml = XDocument.Load(script);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Erro na definição de XML CORS", new Exception(string.Concat("Não foi possível encontrar ou ocorreu um erro no arquivo XML Cors.xml\n", (ex.InnerException != null) ? ex.InnerException.Message : null)));
                }

                try
                {
                    IEnumerable<XElement> corpoElementos = from pt in xml.Elements(XML_NODE_DEFAULT)
                                                           select pt;

                    for (int indice = 0; indice < corpoElementos.Descendants().Count(); indice++)
                    {
                        XElement elemento = corpoElementos.Descendants().ElementAt(indice);

                        string uri = elemento.Value;
                        bool uriValido = Uri.IsWellFormedUriString(uri, UriKind.Absolute);

                        if (uriValido)
                        {
                            lstURI.Add(uri);
                        }
                    }

                    return lstURI.ToArray();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Erro na definição de URLS CORS", new Exception(string.Concat("Ocorreu um erro na definição de URLS", (ex.InnerException != null) ? ex.InnerException.Message : null)));
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message, ex.InnerException);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro na definição CORS", new Exception((ex.InnerException != null) ? ex.InnerException.Message : null));
            }
        }
        #endregion
    }
}