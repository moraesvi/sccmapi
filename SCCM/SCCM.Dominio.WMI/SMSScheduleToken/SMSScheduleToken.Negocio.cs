﻿using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.WMI
{
    public partial class SMSScheduleToken
    {
        public string PsInstance()
        {
            string psInstance = SCCMUteis.PSInstance(this.PSClasse, this);

            return psInstance;
        }

        public string PSInstance()
        {
            string psInstance = SCCMUteis.PSInstance(this.PSClasse, this);

            return psInstance;
        }
    }
}
