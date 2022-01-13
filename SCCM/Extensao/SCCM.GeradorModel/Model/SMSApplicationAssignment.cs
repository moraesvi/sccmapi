using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel.Model
{
    public class SMSApplicationAssignment : WMIObjetoBase
    {
        public SMSApplicationAssignment(string query)
            : base(query, false)
        {

        }
        public SMSApplicationAssignment(string query, bool newtonsoftJsonSerialization)
            : base(query, newtonsoftJsonSerialization)
        {

        }
    }
}
