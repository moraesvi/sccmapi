using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Model.SMSCollectionModel.SMSCollectionUser
{
    public class UserCollection
    {
        [JsonProperty]
        public string UserName { get; set; }
        [JsonProperty]
        public string UniqueUserName { get; set; }
    }
}
