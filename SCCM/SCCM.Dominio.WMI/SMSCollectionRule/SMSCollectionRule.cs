using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SCCM.Dominio.WMI
{
    public partial class SMSCollectionRule
    {
        private string _nomeRegra;
        public SMSCollectionRule(string nomeRegra)
        {
            _nomeRegra = nomeRegra;
        }
        //TODO: Corrigir verificando base na serialização para Powershell
        //public string RuleName
        //{
            //get { return _nomeRegra; }
        //}
    }
}