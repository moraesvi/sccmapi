using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel.Model
{
    public class Win32ComputerSystem : WMIObjetoBase
    {
        private const string QUERY = "Get-WmiObject -Query \"Select * From Win32_ComputerSystem\" | Get-Member -MemberType Property";
        public Win32ComputerSystem()
            : base(QUERY, false)
        {

        }
        public Win32ComputerSystem(bool newtonsoftJsonSerialization)
            : base(QUERY, newtonsoftJsonSerialization)
        {

        }
    }
}
