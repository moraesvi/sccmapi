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
    public partial class SMSRUserAppStatusDeployment : WMIBase<SMSRUserAppStatusDeployment>
    {
        private static List<Chave> param = new List<Chave>()
        {
            new Chave() { IdChave = 1, Nome = "Usuario" },
            new Chave() { IdChave = 2, Nome = "IdApp" }
        };

        public SMSRUserAppStatusDeployment(string usuario, string idApp)
            : base(new PSComando("UsuarioAppStatusImplantacao-Audit", "UsuarioAppStatusImplantacao-Audit.ps1", param, true), new List<ValorChave>()
            {
                new ValorChave() { IdChave = 1, Valor = SCCMHelper.FormatDominioUsuarioPSParam(usuario) },
                new ValorChave() { IdChave = 2, Valor = idApp }
            })
        {

        }
        [JsonProperty]
        public string User { get; set; }
        [JsonProperty]
        public string MachineName { get; set; }
        [JsonProperty]
        public string SoftwareName { get; set; }
        [JsonProperty]
        public string ComplianceState { get; set; }
        [JsonProperty]
        public string State { get; set; }
        [JsonProperty]
        public string StateDetail { get; set; }
        [JsonProperty]
        public UInt32 ErrorCode { get; set; }
        [JsonProperty]
        public string ErrorMessage { get; set; }
        [JsonProperty]
        public DateTime StartTime { get; set; }
    }
}
