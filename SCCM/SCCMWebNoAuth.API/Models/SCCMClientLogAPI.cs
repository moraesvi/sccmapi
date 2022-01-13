using SCCMWebNoAuth.API.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SCCMWebNoAuth.API.Models
{
    public class SCCMClientLogAPI
    {
        private string _usuario;
        private string _pc;
        private string _dataLog;
        private string _versaoWindows;
        private bool? _windows64Bits;
        private string _caminhoApp;
        private Exception _logException;
        public SCCMClientLogAPI()
        {
            
        }
        public string Usuario
        {
            get { return _usuario; }
            set { _usuario = value; }
        }
        public string PC
        {
            get { return _pc; }
            set { _pc = value; }
        }
        public string DataLog
        {
            get { return _dataLog; }
            set { _dataLog = value; }
        }
        public string VersaoWindows
        {
            get { return _versaoWindows; }
            set { _versaoWindows = value; }
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
            set { _logException = value; }
        }
    }
}