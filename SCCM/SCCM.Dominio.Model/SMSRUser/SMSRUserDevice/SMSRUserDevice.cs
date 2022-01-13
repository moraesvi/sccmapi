using SCCM.Dominio.Comum;
using SCCM.Dominio.Model.SMSRUser.SMSRUserDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Model
{
    public partial class SMSRUserDevice : WMIBase<SMSRUserDevice>
    {
        private static List<Chave> param = new List<Chave>()
        {
            new Chave() { IdChave = 1, Nome = "Usuario" },
            new Chave() { IdChave = 2, Nome = "IdColecao" }
        };
        public SMSRUserDevice(string dominio, string usuario, string idColecao)
            : base(new PSComando("Usuario-Dispositivo", "Usuario-Dispositivo.ps1", param, true), new List<ValorChave>()
            {
                new ValorChave() { IdChave = 1, Valor = SCCMHelper.FormatDominioUsuarioPSParam(dominio, usuario) },
                new ValorChave() { IdChave = 2, Valor = idColecao }
            })
        {

        }
        public UInt32 ResourceID { get; set; }
        public string Name { get; set; }
        public string UniqueUserName { get; set; }
        public UserDevice[] Device { get; set; }
    }
}
