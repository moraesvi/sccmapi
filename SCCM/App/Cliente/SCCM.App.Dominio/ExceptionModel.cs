using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.App.Dominio
{
    public class ExceptionModel
    {
        private string _msg;
        private string _msgDetalhado;
        //private Log log = null;
        public ExceptionModel(string msg, string msgDetalhado, bool gerarLog = false)
        {
            _msg = msg;
            _msgDetalhado = msgDetalhado;

            if (gerarLog && (!string.IsNullOrEmpty(msg) || !string.IsNullOrEmpty(msgDetalhado)))
            {
                //log = new Log("SCCM-API-NoAuth");
                //Log();
            }
        }
        public string Msg
        {
            get { return _msg; }
        }
        public string MsgDetalhado
        {
            get { return _msgDetalhado; }
        }
        public void Log()
        {
            //string erroLogMsg = string.Concat(_msg, "\n\n", _msgDetalhado);

            //log.Novo(erroLogMsg);
        }
    }
}
