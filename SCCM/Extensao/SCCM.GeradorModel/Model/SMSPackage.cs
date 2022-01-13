﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel.Model
{
    public class SMSPackage : WMIObjetoBase
    {
        private const string QUERY = "Get-WmiObject -NameSpace \"ROOT\\SMS\\site_PR1\"  -ClassName SMS_Package | Get-Member -MemberType Property";
        public SMSPackage()
            : base(QUERY, false)
        {

        }
        public SMSPackage(bool newtonsoftJsonSerialization)
            : base(QUERY, newtonsoftJsonSerialization)
        {

        }
    }
}
