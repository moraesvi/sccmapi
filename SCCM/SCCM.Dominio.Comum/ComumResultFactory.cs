using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Comum
{
    public class ComumResultFactory : IComumResult 
    {
        private static bool _executado;
        private static string _msgResult;
        private static object _resultado;
        private static DateTime _dataProc;

        private static ExceptionModel _exception;

        public ComumResultFactory()
        {
            Limpar();
        }

        public static IComumResult Criar(bool executado, string msgResult)
        {
            ComumResultFactory result = new ComumResultFactory();
            _executado = executado;
            _msgResult = msgResult;
            _dataProc = DateTime.Now;

            return result; 
        }
        public static IComumResult Criar(string msgResult)
        {
            ComumResultFactory result = new ComumResultFactory();
            _executado = true;
            _msgResult = msgResult;
            _dataProc = DateTime.Now;

            return result;
        }
        public static IComumResult Criar(bool executado, string msgResult, object resultado)
        {
            ComumResultFactory result = new ComumResultFactory();
            _executado = executado;
            _msgResult = msgResult;
            _dataProc = DateTime.Now;
            _resultado = resultado;

            return result;
        }
        public static IComumResult Criar<T>(string msgResult, T resultado)
        {
            ComumResultFactory result = new ComumResultFactory();
            _executado = true;
            _msgResult = msgResult;
            _dataProc = DateTime.Now;
            _resultado = resultado;

            return result;
        }
        public static IComumResult Criar<T>(bool executado, string msgException, string innerException, string stackTrace, bool debug = false) 
        {
            ComumResultFactory result = new ComumResultFactory();
            _executado = executado;
            _exception = new ExceptionModel(msgException, innerException, stackTrace, debug);
            _dataProc = DateTime.Now;

            return result;
        }
        public static IComumResult Criar<T>(string msgException, string innerException, string stackTrace, bool debug = false) 
        {
            ComumResultFactory result = new ComumResultFactory();
            _executado = false;
            _exception = new ExceptionModel(msgException, innerException, stackTrace, debug);
            _dataProc = DateTime.Now;

            return result;
        }
        public static IComumResult Criar(bool executado, string msgResult, string msgException, string innerException, string stackTrace, bool debug = false)
        {
            ComumResultFactory result = new ComumResultFactory();
            _executado = executado;
            _msgResult = msgResult;
            _dataProc = DateTime.Now;
            _exception = new ExceptionModel(msgException, innerException, stackTrace, debug);

            return result;
        }
        public static IComumResult Criar(string msgResult, string msgException, string innerException, string stackTrace, bool debug = false)
        {
            ComumResultFactory result = new ComumResultFactory();
            _executado = false;
            _msgResult = msgResult;
            _dataProc = DateTime.Now;
            _exception = new ExceptionModel(msgException, innerException, stackTrace, debug);

            return result;
        }
        public static IComumResult Criar<T>(bool executado, T resultado, string msgException, string innerException, string stackTrace, bool debug = false) 
        {
            ComumResultFactory result = new ComumResultFactory();
            _executado = executado;
            _resultado = resultado;
            _dataProc = DateTime.Now;
            _exception = new ExceptionModel(msgException, innerException, stackTrace, debug);

            return result;
        }
        public static IComumResult Criar<T>(T resultado, string msgException, string innerException, string stackTrace, bool debug = false) 
        {
            ComumResultFactory result = new ComumResultFactory();
            _executado = false;
            _resultado = resultado;
            _dataProc = DateTime.Now;
            _exception = new ExceptionModel(msgException, innerException, stackTrace, debug);

            return result;
        }
        public static IComumResult Criar<T>(bool executado, string msgResult, T resultado, string msgException, string innerException, string stackTrace, bool debug = false)
        {
            ComumResultFactory result = new ComumResultFactory();
            _executado = executado;
            _msgResult = msgResult;
            _resultado = resultado;
            _dataProc = DateTime.Now;
            _exception = new ExceptionModel(msgException, innerException, stackTrace, debug);

            return result;
        }
        public static IComumResult Criar<T>(string msgResult, T resultado, string msgException, string innerException, string stackTrace, bool debug = false)
        {
            ComumResultFactory result = new ComumResultFactory();
            _executado = false;
            _msgResult = msgResult;
            _resultado = resultado;
            _dataProc = DateTime.Now;
            _exception = new ExceptionModel(msgException, innerException, stackTrace, debug);

            return result;
        }
        public bool Executado
        {
            get
            {
                return _executado;
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
}
