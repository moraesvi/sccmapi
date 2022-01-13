using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AD.Dominio
{
    public class ExceptionResult
    {
        private string _msg;
        private string _msgDetalhado;
        private string _stackTrace;
        private bool _gerarDetalhe;
        public ExceptionResult(string msg, string msgDetalhado, string stackTrace, bool debug = false)
        {
            _msg = msg;
            _msgDetalhado = msgDetalhado;
            _stackTrace = stackTrace;
            _gerarDetalhe = debug ? true : false;
        }
        public string Msg
        {
            get { return _msg; }
        }
        public string MsgDetalhado
        {
            get
            {
                if (_gerarDetalhe)
                    return _msgDetalhado;

                return string.Empty;
            }
        }
        public string StackTrace
        {
            get
            {
                if (_gerarDetalhe)
                    return _stackTrace;

                return string.Empty;
            }
        }
        public void NaoExibirDetalhe()
        {
            _gerarDetalhe = false;
        }
    }
}
