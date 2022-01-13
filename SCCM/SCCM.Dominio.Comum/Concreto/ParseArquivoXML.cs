using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SCCM.Dominio.Comum
{
    public class ParseArquivoXML : IParseArquivo
    {
        private const string XML_CORPO = "CORPO";
        private const string XML_CHAVE = "CHAVE";
        private const string XML_DISPOSITIVO = "DISPOSITIVO";
        private const string XML_COMANDO_LINHA = "SCRIPT-LINHA-";

        private const string XML_LINHA_CARACTER_ESPECIAL = "'";

        private const string EXTENSAO_ARQUIVO = "XML";

        private string _xmlResult;
        private string _dispositivo;
        private List<string> _lstLinhas;
        public ParseArquivoXML(string dispositivo)
        {
            _lstLinhas = new List<string>();
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
            GerarCorpoXML(linha);
        }
        public string Obter(string chave, string[] linhas)
        {
            Encoding utf8noBOM = new UTF8Encoding(false);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = utf8noBOM;

            using (MemoryStream output = new MemoryStream())
            {
                using (XmlWriter xml = XmlWriter.Create(output, settings))
                {
                    GerarCabecalhoXML(xml, chave);
                    GerarCorpoXML(xml, linhas);
                    xml.WriteEndElement();
                    xml.Flush();
                }

                _xmlResult = Encoding.UTF8.GetString(output.ToArray());
            }

            return _xmlResult;
        }
        public string Obter(string chave)
        {
            Encoding utf8noBOM = new UTF8Encoding(false);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = utf8noBOM;

            using (MemoryStream output = new MemoryStream())
            {
                using (XmlWriter xml = XmlWriter.Create(output, settings))
                {
                    GerarCabecalhoXML(xml, chave);
                    GerarCorpoXML(xml, _lstLinhas.ToArray());
                    xml.WriteEndElement();
                    xml.Flush();
                }

                _xmlResult = Encoding.UTF8.GetString(output.ToArray());
            }

            return _xmlResult;
        }

        #region Metodos Privados
        private void GerarCabecalhoXML(XmlWriter xml, string chave)
        {
            xml.WriteStartDocument();
            xml.WriteStartElement(XML_CORPO);
            xml.WriteAttributeString(XML_CHAVE, "chave-busca");
            xml.WriteElementString(XML_CHAVE, chave);
            xml.WriteElementString(XML_DISPOSITIVO, _dispositivo);
        }
        private void GerarCorpoXML(XmlWriter xml, string[] linhas)
        {
            if (linhas == null || linhas.Count() == 0)
            {
                xml.WriteElementString(string.Concat(XML_COMANDO_LINHA, 1), "Não existe um comando válido.");
                return;
            }

            linhas = linhas.Where(valor => !string.IsNullOrWhiteSpace(valor))
                           .ToArray();

            for (int indice = 0; indice < linhas.Count(); indice++)
            {
                string linhaComandoXML = string.Concat(XML_COMANDO_LINHA, indice);
                string valor = linhas.ElementAtOrDefault(indice);

                valor = valor.Trim();
                valor = valor.Contains(XML_LINHA_CARACTER_ESPECIAL) && valor.Contains("\"\"") ? valor.Replace(XML_LINHA_CARACTER_ESPECIAL, "\"\"") : valor;

                xml.WriteElementString(string.Concat(XML_COMANDO_LINHA, (indice + 1)), valor);
            }
        }
        private void GerarCorpoXML(string linha)
        {
            _lstLinhas.Add(linha);
        }
        #endregion
    }
}
