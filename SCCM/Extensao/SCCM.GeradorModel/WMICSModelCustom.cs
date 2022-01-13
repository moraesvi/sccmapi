using SCCM.GeradorModel.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel
{
    public class WMICSModelCustom
    {
        public string Gerar(bool newtonsoftJsonSerialization = true)
        {
            //using (PSConexao objConexao = new PSConexao("PRBBR\\xb194031", "mtp@2134", "SRVSRVCPVWBR05.prbbr.produbanbr.corp"))
            using (PSConexao objConexao = new PSConexao("ADTeste\\sccm_admin", "mtp@1234", "SCCM"))
            {
                List<IObjetoWMI> lstConcreto = new List<IObjetoWMI>()
                {
                    new ModelCustom.SMSRSystemCustom()
                };

                List<string> lstMsgResult = new List<string>();

                List<WMIModelResult> lstModelSMSCollectionResult = objConexao.ObterModel(lstConcreto);

                lstModelSMSCollectionResult.ForEach(model =>
                {
                    string csModel = WMICSModelTemplate.CSModelTemplate(model.NomeClasse, model.Prop, newtonsoftJsonSerialization);
                    string msgArquivoCS = WMICSModelTemplate.GerarArquivoCS(model.NomeClasse, csModel, "ModelCustom");

                    lstMsgResult.Add(msgArquivoCS);
                });

                return lstMsgResult.FirstOrDefault();
            }
        }

        public string ObterQueryPS(string query)
        {
            string queryPS = string.Format("Get-WmiObject -NameSpace \"ROOT\\SMS\\site_PR1\" -Query \"{0}\" | Get-Member -MemberType Property", query);

            return queryPS;
        }
    }
}
