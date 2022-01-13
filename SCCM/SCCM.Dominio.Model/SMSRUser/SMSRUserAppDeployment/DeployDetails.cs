using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Model.SMSRUser.SMSRUserComputerAppDeployment
{
    public class DeployDetails
    {
        [JsonProperty]
        public string ComplianceState { get; set; }
        [JsonProperty]
        public string MachineName { get; set; }
        [JsonProperty]
        public string State { get; set; }
        [JsonProperty]
        public string StateDetail { get; set; }
        [JsonProperty]
        public UInt32 ErrorCode { get; set; }
        [JsonProperty]
        public string ErrorMessage { get; set; }
        [JsonProperty]
        public string LastComplianceStateChange { get; set; }
    }
}
