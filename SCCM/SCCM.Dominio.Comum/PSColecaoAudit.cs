using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Comum
{
    public class PSColecaoAudit
    {
        private const string PREF_COLECAO = "SMS_Collection.CollectionID=";

        private string _relativePath;

        public string RelativePath
        {
            get { return _relativePath; }
            set { _relativePath = value; }
        }
        public string CollectionId()
        {
            string collectionId = _relativePath.Replace(PREF_COLECAO, "").Replace("\"", "").Trim();
            return collectionId;
        }
        public bool Inserido()
        {
            string collectionId = _relativePath.Replace(PREF_COLECAO, "").Replace("\"", "").Trim();

            if (!string.IsNullOrWhiteSpace(collectionId))
                return true;

            return false;
        }
    }
}
