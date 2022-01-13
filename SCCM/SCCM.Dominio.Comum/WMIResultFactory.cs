using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Comum
{
    public class WMIResultFactory : IWMIResult
    {
        private static bool _executado;
        private static string _dominio;
        private static string _msgResult;
        private static object _resultado;
        private static DateTime _dataProc;

        private static ExceptionModel _exception;

        public WMIResultFactory()
        {
            Limpar();
        }
        public static IWMIResult Criar(bool executado, string msgResult, string dominio)
        {
            WMIResultFactory result = new WMIResultFactory();
            _executado = executado;
            _dominio = dominio;
            _msgResult = msgResult;
            _dataProc = DateTime.Now;

            return result;
        }
        public static IWMIResult Criar(string msgResult, string dominio)
        {
            WMIResultFactory result = new WMIResultFactory();
            _executado = true;
            _dominio = dominio;
            _msgResult = msgResult;
            _dataProc = DateTime.Now;

            return result;
        }
        public static IWMIResult Criar(bool executado, string msgResult, string dominio, object resultado)
        {
            WMIResultFactory result = new WMIResultFactory();
            _executado = executado;
            _dominio = dominio;
            _msgResult = msgResult;
            _dataProc = DateTime.Now;
            _resultado = resultado;

            return result;
        }
        public static IWMIResult Criar(string msgResult, string dominio, object resultado)
        {
            WMIResultFactory result = new WMIResultFactory();
            _executado = true;
            _dominio = dominio;
            _msgResult = msgResult;
            _dataProc = DateTime.Now;
            _resultado = resultado;

            return result;
        }
        public static IWMIResult Criar(bool executado, string dominio, string msgException, string innerException, string stackTrace, bool debug = false)
        {
            WMIResultFactory result = new WMIResultFactory();
            _executado = executado;
            _dominio = dominio;
            _exception = new ExceptionModel(msgException, innerException, stackTrace, debug);
            _dataProc = DateTime.Now;

            return result;
        }
        public static IWMIResult Criar(string dominio, string msgException, string innerException, string stackTrace, bool debug = false)
        {
            WMIResultFactory result = new WMIResultFactory();
            _executado = false;
            _dominio = dominio;
            _exception = new ExceptionModel(msgException, innerException, stackTrace, debug);
            _dataProc = DateTime.Now;

            return result;
        }
        public static IWMIResult Criar(bool executado, string dominio, string msgResult, string msgException, string innerException, string stackTrace, bool debug = false)
        {
            WMIResultFactory result = new WMIResultFactory();
            _executado = executado;
            _dominio = dominio;
            _msgResult = msgResult;
            _dataProc = DateTime.Now;
            _exception = new ExceptionModel(msgException, innerException, stackTrace, debug);

            return result;
        }
        public static IWMIResult Criar(string dominio, string msgResult, string msgException, string innerException, string stackTrace, bool debug = false)
        {
            WMIResultFactory result = new WMIResultFactory();
            _executado = false;
            _dominio = dominio;
            _msgResult = msgResult;
            _dataProc = DateTime.Now;
            _exception = new ExceptionModel(msgException, innerException, stackTrace, debug);

            return result;
        }
        public static IWMIResult Criar(bool executado, string dominio, object resultado, string msgException, string innerException, string stackTrace, bool debug = false) 
        {
            WMIResultFactory result = new WMIResultFactory();
            _executado = executado;
            _dominio = dominio;
            _resultado = resultado;
            _dataProc = DateTime.Now;
            _exception = new ExceptionModel(msgException, innerException, stackTrace, debug);

            return result;
        }
        public static IWMIResult Criar(string dominio, object resultado, string msgException, string innerException, string stackTrace, bool debug = false) 
        {
            WMIResultFactory result = new WMIResultFactory();
            _executado = false;
            _dominio = dominio;
            _resultado = resultado;
            _dataProc = DateTime.Now;
            _exception = new ExceptionModel(msgException, innerException, stackTrace, debug);

            return result;
        }
        public static IWMIResult Criar(bool executado, string dominio, string msgResult, object resultado, string msgException, string innerException, string stackTrace, bool debug = false) 
        {
            WMIResultFactory result = new WMIResultFactory();
            _executado = executado;
            _dominio = dominio;
            _msgResult = msgResult;
            _resultado = resultado;
            _dataProc = DateTime.Now;
            _exception = new ExceptionModel(msgException, innerException, stackTrace, debug);

            return result;
        }
        public static IWMIResult Criar(string dominio, string msgResult, object resultado, string msgException, string innerException, string stackTrace, bool debug = false)
        {
            WMIResultFactory result = new WMIResultFactory();
            _executado = false;
            _dominio = dominio;
            _msgResult = msgResult;
            _resultado = resultado;
            _dataProc = DateTime.Now;
            _exception = new ExceptionModel(msgException, innerException, stackTrace, debug);

            return result;
        }
        public static IWMIResult SerializarResult<TResult>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new System.Exception("Operação inválida", new System.Exception("Json de serialização null ou vazio"));
            }

            WMIResultFactory result = new WMIResultFactory();

            ModelResult<TResult> modelResult = JsonConvert.DeserializeObject<ModelResult<TResult>>(json);

            _executado = modelResult.Executado;
            _dominio = modelResult.Domain;
            _msgResult = modelResult.MsgResul;
            _resultado = modelResult.Result;
            _dataProc = Convert.ToDateTime(modelResult.Data);

            if (modelResult.Exception != null)
                _exception = new ExceptionModel(modelResult.Exception.Msg, modelResult.Exception.MsgDetalhado, null, false);

            return result;
        }        
        public bool Executado
        {
            get
            {
                return _executado;
            }
        }
        public string Domain
        {
            get
            {
                return _dominio;
            }
        }
        public string MsgResul
        {
            get
            {
                return _msgResult;
            }
        }
        public object Result
        {
            get
            {
                return _resultado;
            }
        }
        public string Data
        {
            get
            {
                return _dataProc.ToString("dd/MM/yyyy HH:mm");
            }
        }
        public ExceptionModel Exception
        {
            get
            {
                return _exception;
            }
        }

        private void Limpar()
        {
            _executado = false;
            _msgResult = string.Empty;
            _resultado = null;
            _exception = null;
        }
    }
    internal class ModelResult<TResult>
    {
        public bool Executado { get; set; }
        public string Domain { get; set; }
        public string MsgResul { get; set; }
        public string Data { get; set; }
        public TResult Result { get; set; }
        public ExceptionResult Exception { get; set; }
    }
    public class ExceptionResult
    {
        public string Msg { get; set; }
        public string MsgDetalhado { get; set; }
    }
}
