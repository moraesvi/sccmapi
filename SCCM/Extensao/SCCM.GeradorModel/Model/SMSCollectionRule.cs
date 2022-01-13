using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel.Model
{
    public class SMSCollectionRule : WMIObjetoBase
    {
        public SMSCollectionRule(string query)
            : base(query, false)
        {

        }
        public SMSCollectionRule(string query, bool newtonsoftJsonSerialization)
            : base(query, newtonsoftJsonSerialization)
        {

        }
    }
}
