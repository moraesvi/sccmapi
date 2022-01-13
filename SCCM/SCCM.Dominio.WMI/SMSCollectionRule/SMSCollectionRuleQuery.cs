using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.WMI
{
    public partial class SMSCollectionRuleQuery : SMSCollectionRule
    {
        private PSClasse _PSClasse;
        private string _query;
        private string _nomeRegra;
        public SMSCollectionRuleQuery(string nomeRegra, string query)
            : base(nomeRegra)
        {
            _query = query;
            _nomeRegra = nomeRegra;
            _PSClasse = new PSClasse(SCCMHelper.SMSSiteNamespace, "SMS_CollectionRuleQuery");

            ValidarQuery();
        }
        internal PSClasse PSClasse
        {
            get { return _PSClasse; }
        }
        public string LimitToCollectionID { get; set; }
        //TODO: Alterar para buscar da classe base.
        public string RuleName
        {
            get { return _nomeRegra; }
        }
        public string QueryExpression
        {
            get { return _query; }
        }
        public UInt32 QueryID { get; set; }
    }
}
