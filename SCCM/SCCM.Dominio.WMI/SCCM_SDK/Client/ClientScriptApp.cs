using Newtonsoft.Json;
using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.WMI
{
    [JsonObject(MemberSerialization.OptIn)]
    public partial class ClientScriptApp : WMIBase<ClientScriptApp>
    {
        private static List<Chave> param = new List<Chave>()
        {
            new Chave() { IdChave = 1, Nome = "AppId" }
        };

        private string _appId;

        public ClientScriptApp(string appId)
            : base(new PSComando("ClientSDK-InstalacaoApp", "ClientSDK-InstalacaoApp.ps1", param, true), new List<ValorChave>()
            {
                new ValorChave() { IdChave = 1, Valor = appId }
            })
        {
            _appId = appId;
        }
        [JsonProperty]
        public string AppId
        {
            get { return _appId; }
            set { _appId = value; }
        }
        [JsonProperty]
        public byte[] Script { get; set; }
    }
}
