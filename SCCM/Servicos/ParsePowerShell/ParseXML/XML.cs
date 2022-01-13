using SCCM.Servico.Contratos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ParseXML
{
    public class XML : IParsePowerShell
    {
        private const string XML_CORPO = "CORPO";
        private const string XML_CHAVE = "CHAVE";
        private const string XML_DISPOSITIVO = "DISPOSITIVO";
        private const string XML_COMANDO_LINHA = "SCRIPT-LINHA-";

        private const string EXTENSAO_ARQUIVO = "XML";

        private string _chave;
        private string _dispositivo;
        private string _extensao;
        public string Chave
        {
            get
            {
                return _chave;
            }
        }
        public string Dispositivo
        {
            get
            {
                return _dispositivo;
            }
        }
        public string ExtensaoArquivo
        {
            get
            {
                return EXTENSAO_ARQUIVO;
            }
        }
        public string ObterComandoPS(string arquivoCaminhoCompleto)
        {
            if (string.IsNullOrEmpty(arquivoCaminhoCompleto))
                throw new InvalidOperationException("Arquivo inválido", new Exception("É obrigatório inserir um arquivo."));

            StringBuilder sbComando = new StringBuilder();
            XDocument xml = XDocument.Load(arquivoCaminhoCompleto);

            IEnumerable<XElement> corpoElementos = xml.Descendants(XML_CORPO);

            _chave = corpoElementos.SingleOrDefault().Element(XML_CHAVE).Value;
            _dispositivo = corpoElementos.SingleOrDefault().Element(XML_DISPOSITIVO).Value;

            for (int indiceElemento = 0; indiceElemento < corpoElementos.Descendants().Count(); indiceElemento++)
            {
                string elementoLinha = string.Concat(XML_COMANDO_LINHA, indiceElemento);

                XElement elemento = corpoElementos.SingleOrDefault().Element(elementoLinha);

                if (elemento != null)
                {
                    string comando = corpoElementos.SingleOrDefault().Element(elementoLinha).Value;

                    sbComando.AppendLine(comando);
                }
            }

            return sbComando.ToString();
        }
        public bool Validar(string arquivoCaminhoCompleto)
        {
            if (string.IsNullOrEmpty(arquivoCaminhoCompleto))
                throw new InvalidOperationException("Arquivo inválido", new Exception("É obrigatório inserir um arquivo."));

            bool valido = true;

            XDocument xml = XDocument.Load(arquivoCaminhoCompleto);

            IEnumerable<XElement> corpoElementos = xml.Descendants(XML_CORPO);

            string arquivo = Path.GetFileName(arquivoCaminhoCompleto);

            if (corpoElementos == null || corpoElementos.Count() == 0)
            {
                throw new InvalidOperationException("XML inválido", new Exception(string.Concat("Corpo do XML vazio.\n\n", arquivo)));
            }

            if (!corpoElementos.SingleOrDefault().HasElements)
            {
                throw new InvalidOperationException("XML inválido", new Exception(string.Concat("Corpo do XML vazio.\n\n", arquivo)));
            }

            XElement elementoChave = corpoElementos.SingleOrDefault().Element(XML_CHAVE);
            XElement elementoDisp = corpoElementos.SingleOrDefault().Element(XML_DISPOSITIVO);

            valido = (!string.IsNullOrWhiteSpace(elementoChave.Value)) && (!string.IsNullOrWhiteSpace(elementoDisp.Value));

            if (!valido)
                return false;

            IEnumerable<XElement> linhaElementos = corpoElementos.Descendants();

            if (linhaElementos == null && linhaElementos.Count() == 0)
            {
                throw new InvalidOperationException("XML inválido", new Exception(string.Concat("Corpo do XML vazio, não possui comando PowerShell.\n\n", arquivo)));
            }

            for (int indiceElemento = 1; indiceElemento <= linhaElementos.Count(); indiceElemento++)
            {
                string elementoLinha = string.Concat(XML_COMANDO_LINHA, indiceElemento);

                int total = corpoElementos.Elements().Where(valor => valor.Element(elementoLinha) != null)
                                          .Count();

                if(total > 1)
                {
                    throw new InvalidOperationException("XML inválido", new Exception(string.Concat("Corpo do XML vazio, possúi elementos repetidos.\n\n", arquivo)));
                }

                XElement elemento = corpoElementos.SingleOrDefault().Element(elementoLinha);

                if (elemento != null)
                {
                    valido = (!string.IsNullOrWhiteSpace(corpoElementos.SingleOrDefault().Element(elementoLinha).Value));

                    if (!valido)
                        break;
                }
            }

            return valido;
        }
        public bool ExtensaoValido(string arquivoCaminhoCompleto)
        {
            string extensao = Path.GetExtension(arquivoCaminhoCompleto);

            if (extensao.IndexOf(EXTENSAO_ARQUIVO, StringComparison.OrdinalIgnoreCase) < 0)
            {
                throw new InvalidOperationException("Arquivo inválido", new Exception("O arquivo não é um XML"));
            }

            _extensao = extensao;

            return true;
        }
        private string RemoveCaracteresInvalidos(string text)
        {
            string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            return System.Text.RegularExpressions.Regex.Replace(text, re, "");
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
