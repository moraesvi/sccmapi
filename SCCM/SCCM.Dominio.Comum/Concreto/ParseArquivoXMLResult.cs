using Newtonsoft.Json;
using SCCM.Dominio.Comum.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SCCM.Dominio.Comum.Concreto
{
    public class ParseXMLArquivoJSONResult : IParseArquivoResult
    {
        public bool Valido(string textoXml)
        {
            bool valido = false;

            if (!string.IsNullOrEmpty(textoXml) && textoXml.TrimStart().StartsWith("<"))
            {
                try
                {
                    var doc = XDocument.Parse(textoXml);
                    valido = true;
                }
                catch 
                {
                    valido = false;
                }
            }

            return valido;
        }
        public string ParseResult(string textoXml)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(textoXml);

            string json = JsonConvert.SerializeXmlNode(xml);

            return json;
        }
    }
}
