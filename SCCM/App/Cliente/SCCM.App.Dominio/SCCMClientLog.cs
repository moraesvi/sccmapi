using System;
using System.Text;
using HelperComum;

namespace SCCM.App.Dominio
{
    public class SCCMClientLog
    {
        private string _usuario;
        private string _pc;
        private DateTime _dataLog;
        private string _versaoWindows;
        private bool? _windows64Bits;
        private string _caminhoApp;
        private Exception _logException;
        public SCCMClientLog()
        {
            _usuario = "Não foi possível definir um usuário";
            _pc = "Não foi possível definir o host";
            _dataLog = DateTime.Now;
            _versaoWindows = "Não foi possível definir a versão do windows";
            _windows64Bits = null;
            _caminhoApp = System.AppDomain.CurrentDomain.BaseDirectory;
        }
        public SCCMClientLog(string usuario, string pc, DateTime dataLog, string versaoWindows, bool? windows64Bits, string caminhoApp)
        {
            _usuario = usuario;
            _pc = pc;
            _dataLog = dataLog;
            _versaoWindows = versaoWindows;
            _windows64Bits = windows64Bits;
            _caminhoApp = caminhoApp;
        }
        public string Usuario
        {
            get { return _usuario; }
        }
        public string PC
        {
            get { return _pc; }
        }
        public string DataLog
        {
            get { return _dataLog.ToString("dd/MM/yyyy hh:mm:ss"); }
        }
        public string VersaoWindows
        {
            get { return _versaoWindows; }
        }
        public bool? Windows64Bits
        {
            get { return _windows64Bits; }
            set { _windows64Bits = value; }
        }
        public string CaminhoApp
        {
            get { return _caminhoApp; }
            set { _caminhoApp = value; }
        }
        public Exception LogException
        {
            get { return _logException; }
        }
        public void DefinirException(Exception exception)
        {
            if (exception == null)
                return;

            _logException = exception;
        }
        public string LogFormat()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Concat("\rLog - : ", _pc));
            sb.AppendLine(_dataLog.ToLongDateString());
            sb.AppendLine(_dataLog.ToString("dd/MM/yyyy HH:mm"));
            sb.AppendLine("-----------------------------------------------------------------------------------");
            sb.AppendLine(_usuario);
            sb.AppendLine(_versaoWindows);
            sb.AppendLine(string.Concat("64 bits: ", _windows64Bits.HasValue ? "sim" : "não"));
            sb.AppendLine("-----------------------------------------------------------------------------------");
            sb.AppendLine(_caminhoApp);
            sb.AppendLine("-----------------------------------------------------------------------------------");

            return sb.ToString();
        }
    }
}
