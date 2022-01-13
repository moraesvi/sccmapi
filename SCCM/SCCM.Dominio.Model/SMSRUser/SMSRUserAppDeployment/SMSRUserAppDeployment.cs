using Newtonsoft.Json;
using SCCM.Dominio.Comum;
using SCCM.Dominio.Model.SMSRUser.SMSRUserComputerAppDeployment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public partial class SMSRUserAppDeployment : WMIBase<SMSRUserAppDeployment>
    {
        private static List<Chave> param = new List<Chave>()
        {
            new Chave() { IdChave = 1, Nome = "Usuario" },
        };
        public SMSRUserAppDeployment(string usuario)
            : base(new PSComando("UsuarioAppImplantacao-Audit", "UsuarioAppImplantacao-Audit.ps1", param, true), new List<ValorChave>()
            {
                new ValorChave() { IdChave = 1, Valor = usuario }
            })
        {

        }
        [JsonProperty]
        public string CollectionID { get; set; }
        [JsonProperty]
        public string CollectionName { get; set; }
        [JsonProperty]
        public string User { get; set; }
        [JsonProperty]
        public string SoftwareName { get; set; }
        [JsonProperty]
        public DateTime DeploymentTime { get; set; }
        [JsonProperty]
        public string DeploymentIntent { get; set; }
        [JsonProperty]
        public DeployDetails[] DeployDetails { get; set; }
    }
}
