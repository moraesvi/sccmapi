using Microsoft.Win32;
using SCCM.App.Dominio;
using SCCM.App.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.App.Cliente
{
    public class URLProtocol
    {
        private const int TOTAL_TENTATIVAS_SCCM_API = 3;

        private string _nomeProtocolo;
        private string _caminhoAplicacao;
        private string _descricao;

        public URLProtocol(string nomeProtocolo, string caminhoAplicacao)
        {
            _nomeProtocolo = nomeProtocolo.Replace(":", "");
            _caminhoAplicacao = caminhoAplicacao;
        }
        public URLProtocol(string nomeProtocolo, string descricao, string caminhoAplicacao)
        {
            _nomeProtocolo = nomeProtocolo.Replace(":", "");
            _descricao = descricao;
            _caminhoAplicacao = caminhoAplicacao;
        }
        public bool Existe(string chaveBusca = null)
        {
            try
            {
                string nomeProtocoloFormat = string.Empty;

                if (!string.IsNullOrEmpty(chaveBusca))
                    nomeProtocoloFormat = chaveBusca;
                else
                    nomeProtocoloFormat = string.Concat(_nomeProtocolo, "\\Shell\\open\\command");

                RegistryKey rKey = Registry.ClassesRoot.OpenSubKey(nomeProtocoloFormat);

                if (rKey != null)
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro na verificação do Protocolo", ex.InnerException);
            }
        }
        public bool Remover()
        {
            try
            {
                if (Existe(_nomeProtocolo))
                {
                    Registry.ClassesRoot.DeleteSubKey(string.Concat(_nomeProtocolo, "\\Shell\\open\\command"));
                    Registry.ClassesRoot.DeleteSubKey(string.Concat(_nomeProtocolo, "\\Shell\\open"));
                    Registry.ClassesRoot.DeleteSubKey(string.Concat(_nomeProtocolo, "\\Shell"));
                    Registry.ClassesRoot.DeleteSubKey(_nomeProtocolo);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro na verificação do Protocolo", ex.InnerException);
            }
        }
        public bool Registrar()
        {
            try
            {
                RegistryKey rKey = Registry.ClassesRoot.CreateSubKey(_nomeProtocolo);

                rKey.SetValue(null, _descricao);
                rKey.SetValue("URL Protocol", string.Empty);

                Registry.ClassesRoot.CreateSubKey(string.Concat(_nomeProtocolo, "\\Shell"));
                Registry.ClassesRoot.CreateSubKey(string.Concat(_nomeProtocolo, "\\Shell\\open"));
                rKey = Registry.ClassesRoot.CreateSubKey(string.Concat(_nomeProtocolo, "\\Shell\\open\\command"));

                rKey.SetValue(null, string.Concat("\"", _caminhoAplicacao, "\" %1"));

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro na criação do Protocolo", ex.InnerException);
            }
        }
        public SCCMClientProtocolo HttpClientProtocolo()
        {
            try
            {
                IWMIResult wmiResult = null;
                SCCMClientProtocolo sccmClientProtocolo = null;

                string sccmApiURL = ConfigurationManager.AppSettings.Get("SCCM_API_URL");

                sccmApiURL = string.Concat(sccmApiURL, "/api/util/URLProtocolo");

                string json = WebApiHttp.HttpJsonRequisicao(TOTAL_TENTATIVAS_SCCM_API, sccmApiURL);

                if (!string.IsNullOrWhiteSpace(json))
                {
                    wmiResult = WMIResultFactory.SerializarResult<SCCMClientProtocolo>(json);

                    if (!wmiResult.Executado)
                    {
                        throw new InvalidOperationException("Não foi possível buscar o script SCCMClient Protocolo", new Exception(wmiResult.Exception != null ? wmiResult.Exception.MsgDetalhado : null));
                    }
                }
                else
                {
                    throw new InvalidOperationException("Não foi possível buscar script SCCMClient Protocolo", null);
                }

                sccmClientProtocolo = wmiResult.Result as SCCMClientProtocolo;

                return sccmClientProtocolo;
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message, ex.InnerException);
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível realizar a requisição SCCMClient Protocolo", ex.InnerException);
            }
        }
    }
}
