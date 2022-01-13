using Newtonsoft.Json;
using SCCM.Dominio.Comum;
using SCCM.Dominio.Model.SMSCollectionModel.SMSCollectionUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public partial class SMSCollectionUser : WMIBase<SMSCollectionUser>
    {
        private static List<Chave> param = new List<Chave>()
        {
            new Chave() { IdChave = 1, Nome = "idColecao" },
        }; 
        public SMSCollectionUser(string idColecao)
            : base(new PSComando("Colecao-Usuario", "Colecao-Usuario.ps1", param, true), 
                  new List<ValorChave>
                  {
                      new ValorChave() { IdChave = 1, Valor = idColecao } 
                  }
            )
        { 

        }
        [JsonProperty]
        public string CollectionID { get; set; }
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public int MemberCount { get; set; }
        [JsonProperty]
        public UserCollection[] UserCollection { get; set; }
    }
}
