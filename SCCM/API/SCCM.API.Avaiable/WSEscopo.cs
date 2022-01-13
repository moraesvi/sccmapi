using Infraestrutura;
using SCCM.API.Infraestrutura.ApplicationViewService;
using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.API.Avaiable
{
    public class WSEscopo
    {
        internal static void Definir(NetworkCredential crendenciais)
        {
            AcessoWS.Definir(crendenciais);
        }
        internal static void Definir(SCCMDominio dominio)
        {
            AcessoWS.Definir(dominio);
        }
        internal static ApplicationViewServiceSoapClient Obter()
        {
            return AcessoWS.WSAppViewService;
        }
    }
}
