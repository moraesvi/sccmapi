using Newtonsoft.Json;
using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public partial class SMSCollection : WMIBase<SMSCollection>
    {
        private const string QUERY_BASE = "Select CollectionID, CollectionType, Name, Comment, CurrentStatus, MemberCount From SMS_Collection";
        public SMSCollection()
            : base(new PSQuery(SCCMHelper.SMSSiteNamespace, QUERY_BASE), "CollectionID")
        {
            
        }
        [JsonProperty]
        public string CollectionID { get; set; }
        [JsonProperty]
        public UInt32 CollectionType { get; set; }
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public string Comment { get; set; }
        [JsonProperty]
        public UInt32 CurrentStatus { get; set; }
        [JsonProperty]
        public int MemberCount { get; set; }
    }
}
