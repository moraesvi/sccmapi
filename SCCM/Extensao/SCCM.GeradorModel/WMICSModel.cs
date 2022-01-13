using SCCM.GeradorModel.Model;
using acutualConfig = SCCM.GeradorModel.Model.ActualConfig;
using requestedConfig = SCCM.GeradorModel.Model.RequestedConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel
{
    public class WMICSModel
    {
        public string Gerar(bool newtonsoftJsonSerialization = false)
        {
            using (PSConexao objConexao = new PSConexao("PRBBR\\xb194031", "mtp@4433", "SRVSRVCPVWBR05.prbbr.produbanbr.corp"))
            //using (PSConexao objConexao = new PSConexao("ADTeste\\sccm_admin", "mtp@1234", "SCCM"))
            {
                List<string> lstMsgResult = new List<string>();

                List<IObjetoWMI> lstConcreto = new List<IObjetoWMI>()
                {
                    new SMSProviderLocation(),
                };

                List<WMIModelResult> lstModelConcreto = objConexao.ObterModel(lstConcreto);

                lstModelConcreto.ForEach(model =>
                {
                    string csModel = WMICSModelTemplate.CSModelTemplate(model.NomeClasse, model.Prop, newtonsoftJsonSerialization);
                    string msgArquivoCS = WMICSModelTemplate.GerarArquivoCS(model.NomeClasse, csModel, "Model");

                    lstMsgResult.Add(msgArquivoCS);
                });

                return lstMsgResult.FirstOrDefault();
            }
        }        

        public string ObterClasseQuery(string classe)
        {
            string query = string.Format("Get-WmiObject -NameSpace \"ROOT\\SMS\\site_PR1\" -ClassName {0} | Get-Member -MemberType Property", classe);

            return query;
        }

        public string ObterClasseBaseQuery(string classe)
        {
            string query = string.Format("Get-WmiObject {classe} -NameSpace \"ROOT\\SMS\\site_PR1\" | select -First 1 | Get-Member -MemberType Property", classe);

            return query;
        }
    }
}
