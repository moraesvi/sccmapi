using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel.Model
{
    public class SMSDeploymentInfo : WMIObjetoBase
    {
        public SMSDeploymentInfo(string query)
            : base(query, false)
        {

        }
        public SMSDeploymentInfo(string query, bool newtonsoftJsonSerialization)
            : base(query, newtonsoftJsonSerialization)
        {

        }
    }
}
