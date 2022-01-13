using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Comum
{
    public class PSQuery
    {
        private string _classe;
        private string _namespace;
        private string _query;
        private string _objectResult;

        private const string FILRO_WHERE = "WHERE {0} = {1}";
        private const string FILRO_WHERE_STRING = "WHERE {0} = '{1}'";
        private const string FILRO_AND = "AND {0} = {1}";
        private const string FILRO_AND_STRING = "AND {0} = '{1}'";

        public PSQuery(string query)
        {
            _query = query;

            int indexClasse = query.IndexOf("from ", StringComparison.OrdinalIgnoreCase) + 5;
            int indexCompleto = query.Length;

            string smsClasse = query.Substring(indexClasse, (indexCompleto - indexClasse));

            _classe = smsClasse;
        }
        public PSQuery(string wmiNamespace, string query)
        {
            _namespace = wmiNamespace;
            _query = query;

            int indexClasse = query.IndexOf("from ", StringComparison.OrdinalIgnoreCase) + 5;
            int indexCompleto = query.Length;

            string smsClasse = query.Substring(indexClasse, (indexCompleto - indexClasse));

            _classe = smsClasse;
        }
        public PSQuery(string wmiNamespace, string query, string objectResult)
        {
            _namespace = wmiNamespace;
            _query = query;
            _objectResult = objectResult;

            int indexClasse = query.IndexOf("from ", StringComparison.OrdinalIgnoreCase) + 5;
            int indexCompleto = query.Length;

            string smsClasse = query.Substring(indexClasse, (indexCompleto - indexClasse));

            _classe = smsClasse;
        }
        public string Classe
        {
            get { return _classe; }
        }
        public string Namespace
        {
            get { return _namespace; }
        }
        public string Query
        {
            get { return _query; }
        }
        public void AddWhere(string where)
        {
            if (!string.IsNullOrEmpty(where))
            {
                _query = string.Concat(_query, " ", where);
            }
        }
        public void AddFiltro(object param)
        {
            if (!string.IsNullOrEmpty(_query))
            {
                _query = string.Format(_query, param);
            }
        }
        public void AddFiltro(object[] arrayParam)
        {
            if (!string.IsNullOrEmpty(_query))
            {
                arrayParam.ToList().ForEach(param =>
                {
                    _query = string.Format(_query, param);
                });
            }
        }
        public void AddFiltro(string nomeParam, object param)
        {
            if (param.GetType() == typeof(string) || param.GetType().IsClass)
            {
                if (_query.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase) >= 0)
                    _query = string.Format(string.Concat(_query, " ", FILRO_AND_STRING), nomeParam, param);
                else
                    _query = string.Format(string.Concat(_query, " ", FILRO_WHERE_STRING), nomeParam, param);
            }
            else
            {
                if (_query.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase) >= 0)
                    _query = string.Format(string.Concat(_query, " ", FILRO_AND), nomeParam, param);
                else
                    _query = string.Format(string.Concat(_query, " ", FILRO_WHERE), nomeParam, param);

            }
        }
        public override string ToString()
        {
            string PSQuery = string.Empty;

            if (string.IsNullOrEmpty(_objectResult))
            {
                PSQuery = string.Format("Get-WmiObject -Query \"{0}\" -NameSpace \"{1}\"", _query, _namespace);
            }
            else
            {
                PSQuery = string.Format("Get-WmiObject -Query \"{0}\" -NameSpace \"{1}\" | {2}", _query, _namespace, _objectResult);
            }

            return PSQuery;
        }
    }
}
