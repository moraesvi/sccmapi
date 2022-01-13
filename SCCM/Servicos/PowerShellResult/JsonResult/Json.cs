using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SCCM.Servico.Contratos;

namespace JsonResult
{
    public class Json : IPowerShellResult
    {
        public const string EXTENSAO = "JSON";

        private string _chave;

        public string Chave
        {
            get
            {
                return _chave;
            }
        }
        public string ExtensaoArquivo
        {
            get
            {
                return EXTENSAO;
            }
        }
        public string ObterResult(string dispositivo, string chave)
        {
            ModelResult model = new ModelResult();

            GerarCabecalhoJSON(model, dispositivo, chave);
            GerarCorpoJSONVazio(model);

            return ObterJson(model);
        }
        public string ObterResult(string dispositivo, string msgException, string chave)
        {
            ModelResult model = new ModelResult();

            GerarCabecalhoJSON(model, dispositivo, chave);
            GerarCorpoJSONException(model, msgException);

            return ObterJson(model);
        }
        public string ObterResult(ICollection<ErrorRecord> erros, string dispositivo, string chave)
        {
            ModelResult model = new ModelResult();

            GerarCabecalhoJSON(model, dispositivo, chave);
            GerarCorpoJSON(model, erros);

            return ObterJson(model);
        }
        private void GerarCabecalhoJSON(ModelResult model, string dispositivo, string chave)
        {
            model.Dispositivo = dispositivo;
            model.Chave = chave;

            _chave = chave;
        }
        public string ObterResult(ICollection<PSObject> psObjectResults, string dispositivo, string chave)
        {
            PSObject psObject = psObjectResults.Where(t => t != null)
                                               .FirstOrDefault();

            ModelResult model = new ModelResult();

            GerarCabecalhoJSON(model, dispositivo, chave);
            GerarCorpoJSON(model, psObject);

            return ObterJson(model);
        }
        private string ObterJson(ModelResult model)
        {
            string json = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            });

            return json;
        }
        private static void GerarCorpoJSON(ModelResult model, PSObject psObjectResult)
        {
            PSMemberInfoCollection<PSPropertyInfo> propriedades = psObjectResult.Properties;

            StringBuilder sbResult = new StringBuilder();

            sbResult.AppendLine("Script executado com sucesso:");

            for (int indice = 0; indice < propriedades.Count(); indice++)
            {
                PSPropertyInfo psProp = propriedades.ElementAt(indice);

                string valor = (psProp.Value == null) ? string.Empty : psProp.Value.ToString();

                sbResult.AppendLine(valor);
            }

            model.Result = sbResult.ToString();
        }
        private static void GerarCorpoJSON(ModelResult model, ICollection<ErrorRecord> erros)
        {
            StringBuilder sbResult = new StringBuilder();

            sbResult.AppendLine("Script gerou erros na execução");

            for (int indice = 0; indice < erros.Count(); indice++)
            {
                ErrorRecord errorRecord = erros.ElementAt(indice);

                sbResult.AppendLine(errorRecord.ToString());
            }

            model.Result = sbResult.ToString();
        }
        private static void GerarCorpoJSONVazio(ModelResult model)
        {
            StringBuilder sbResult = new StringBuilder();

            sbResult.AppendLine("Script executado com sucesso:");
            sbResult.AppendLine("Não houve resultado PowerShell");

            model.Result = sbResult.ToString();
        }
        private static void GerarCorpoJSONException(ModelResult model, string msgException)
        {
            StringBuilder sbResult = new StringBuilder();

            sbResult.AppendLine("Ocorreu uma exception na execução do script:");
            sbResult.AppendLine(msgException);

            model.Result = sbResult.ToString();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    internal class ModelResult
    {
        public string Chave { get; set; }
        public string Dispositivo { get; set; }
        public string Result { get; set; }
        public string Exception { get; set; }
    }
}
