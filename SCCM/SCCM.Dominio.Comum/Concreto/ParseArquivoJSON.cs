using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Comum
{
    public class ParseArquivoJSON : IParseArquivo
    {
        private const string EXTENSAO_ARQUIVO = "JSON";

        private StringBuilder _sbJSON;
        private ModelJSON _modelJson;
        private string _dispositivo;
        public ParseArquivoJSON(string dispositivo)
        {
            _sbJSON = new StringBuilder();
            _modelJson = new ModelJSON();
            _dispositivo = dispositivo;
        }
        public string Extensao
        {
            get
            {
                return EXTENSAO_ARQUIVO;
            }
        }
        public void Definir(string linha)
        {
            _sbJSON.Append(linha);
        }
        public string Obter(string chave, string[] linhas)
        {
            _sbJSON = new StringBuilder();

            GerarCabecalhoJSON(_modelJson, chave);
            _modelJson.Corpo = GerarCorpoJSON(_sbJSON, linhas);

            return ObterJson(_modelJson);
        }
        public string Obter(string chave)
        {
            GerarCabecalhoJSON(_modelJson, chave);

            if (string.IsNullOrEmpty(_sbJSON.ToString()))
            {
                _sbJSON.Append("Não existe um comando válido");
            }

            _modelJson.Corpo = _sbJSON.ToString();

            return ObterJson(_modelJson);
        }

        #region Metodos Privados
        private string ObterJson(ModelJSON model)
        {
            string json = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            });

            return json;
        }
        private void GerarCabecalhoJSON(ModelJSON model, string chave)
        {
            model.Dispositivo = _dispositivo;
            model.Chave = chave;
        }
        private string GerarCorpoJSON(StringBuilder sbJson, string[] linhas)
        {
            if (linhas == null || linhas.Count() == 0)
            {
                sbJson.Append("Não existe um comando válido.");
                return sbJson.ToString();
            }

            for (int indice = 0; indice < linhas.Count(); indice++)
            {
                string linha = linhas.ElementAtOrDefault(indice);

                sbJson.AppendLine(linha);
            }

            return sbJson.ToString();
        }
        private void GerarCorpoJSON(StringBuilder sbResult, string linha, int indice)
        {
            sbResult.AppendLine(linha);
        }
        #endregion
    }
    internal class ModelJSON
    {
        public string Chave { get; set; }
        public string Dispositivo { get; set; }
        public string Corpo { get; set; }
    }
}
