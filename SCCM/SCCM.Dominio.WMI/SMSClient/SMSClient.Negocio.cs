using HelperComum;
using Newtonsoft.Json;
using SCCM.Dominio.Comum;
using SCCM.Dominio.Comum.Concreto;
using SCCM.Dominio.Comum.Interface;
using SCCM.Dominio.Model;
using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SCCM.Dominio.WMI
{
    public partial class SMSClient
    {
        #region Padrão XML_SCCM

        private const string XML_CORPO = "CORPO";
        private const string XML_CHAVE = "CHAVE";
        private const string XML_DISPOSITIVO = "DISPOSITIVO";
        private const string XML_COMANDO_LINHA = "SCRIPT-LINHA-";

        #endregion

        #region Padrão ARQUIVO_SCCM

        private const string ARQUIVO_PARAM_GERADO = "#PARAMENTRO(S) - ";
        private const string ARQUIVO_COMANDO = "#SCCM_COMANDO";
        private const string ARQUIVO_FUNCTION = "FUNCTION";
        private const string ARQUIVO_FUNCTION_ABRE = "(";
        private const string ARQUIVO_FUNCTION_FECHA = ")";
        private const string ARQUIVO_COMUM_AF = "()";
        private const string ARQUIVO_PARAM = "PARAM";
        private const string ARQUIVO_PARAM_ABRE = "(";
        private const string ARQUIVO_PARAM_PS = "$";
        private const string ARQUIVO_PARAM_SEPARACAO_PS = ",";
        private const string ARQUIVO_PARAM_FECHA = ")";
        private const string ARQUIVO_PARAM_ABRE_CHAVE = "{";
        private const string ARQUIVO_PARAM_FECHA_CHAVE = "}";
        private const string ARQUIVO_PARAM_ABRE_COLCHETE = "[";
        private const string ARQUIVO_PARAM_FECHA_COLCHETE = "]";

        private const string PREF_ARQUIVO_RESULT_NOME = "SCCM_ECOMIGO_Result-";
        private const string PREF_ARQUIVO = "SCCM_ECOMIGO-";

        #endregion

        private const string RESULTADO_COMANDO_PASTA = "PSResultLog";
        private const string SCCM_SERVER = "SCCM_SERVER";

        private string _arquivoCaminhoLocal = Path.Combine(ConfigurationManager.AppSettings.Get("PSDiretorioArquivosLocal"), "PSScripts");
        private string _arquivoCaminhoLocalResult = Path.Combine(ConfigurationManager.AppSettings.Get("PSDiretorioArquivosLocal"), "PSResultados");

        public IComumResult ObterArquivoSCCMResultado(string chaveArquivo)
        {
            string nomeArquivo = string.Concat(PREF_ARQUIVO_RESULT_NOME, chaveArquivo);

            string[] jsonArray = ObterArquivoLocalJSONResult(_arquivoCaminhoLocalResult, nomeArquivo, _parseResultado);

            return ComumResultFactory.Criar(true, string.Format("Resultado buscado do SCCM - Chave: {0}", chaveArquivo), jsonArray);
        }
        public IComumResult GravarArquivoResult(string[] linhasArquivo, string nomeArquivo)
        {
            bool gravado = GravarArquivoLocal(linhasArquivo, _arquivoCaminhoLocal, nomeArquivo);

            if (!gravado)
                return ComumResultFactory.Criar(gravado, string.Format("Não foi possível inserir o arquivo: {0}", nomeArquivo));

            return ComumResultFactory.Criar(gravado, string.Format("Arquivo: {0} salvo na API", nomeArquivo), nomeArquivo);
        }
        public bool GravarArquivo(string[] linhasArquivo, string nomeArquivo)
        {
            bool gravado = GravarArquivoLocal(linhasArquivo, _arquivoCaminhoLocal, nomeArquivo);

            return gravado;
        }
        public IComumResult RemoverArquivoResult(string nomeArquivo)
        {
            bool removido = RemoverArquivoLocal(_arquivoCaminhoLocal, nomeArquivo);

            if (!removido)
                return ComumResultFactory.Criar(removido, string.Format("Não foi possível remover o arquivo: {0}", nomeArquivo));

            return ComumResultFactory.Criar(removido, string.Format("Arquivo: {0} removido da API", nomeArquivo), nomeArquivo);
        }
        public bool RemoverArquivo(string nomeArquivo)
        {
            bool removido = RemoverArquivoLocal(_arquivoCaminhoLocal, nomeArquivo);

            return removido;
        }
        public IComumResult ExecutarComandoResult(PSComandoParam[] comandosParam, string chaveArquivo, string nomeArquivo, string nomeComando)
        {
            bool implantadoSCCM = ExecutarComando(comandosParam, chaveArquivo, nomeArquivo, nomeComando);

            if (!implantadoSCCM)
                return ComumResultFactory.Criar(implantadoSCCM, string.Format("Erro de implantação no SCCM - Arquivo/Comando: {0}/{1}", nomeArquivo, nomeComando));

            return ComumResultFactory.Criar(implantadoSCCM, string.Format("Comando: {0} implantado no SCCM", nomeComando), chaveArquivo);
        }
        public bool ExecutarComando(PSComandoParam[] comandosParam, string chaveArquivo, string nomeArquivo, string nomeComando)
        {
            IParseArquivo parseArquivo = new ParseArquivoXML(SCCM_SERVER);

            string script = string.Empty;
            string arquivo = ObterArquivoLocal(_arquivoCaminhoLocal, nomeArquivo, chaveArquivo, comandosParam, parseArquivo);
            string psArquivo = string.Concat("'", arquivo, "'");

            string caminho = Path.Combine(ConfigurationManager.AppSettings.Get("PSDiretorioArquivos"), ObterArquivoNome(chaveArquivo, parseArquivo.Extensao));

            script = string.Concat(psArquivo, " | Out-File -Encoding \"UTF8\" ", caminho);

            bool implantadoSCCM = PSRequisicao.ExecutarResult(script);

            return implantadoSCCM;
        }
        public IComumResult ExecutarComandoValidarLogadoResult(PSComandoParam[] comandosParam, string chaveArquivo, string idCookie, string dominio, string usuario, string nomeArquivo, string nomeComando, bool gerarNovaThread = false)
        {
            if (gerarNovaThread)
            {
                Task<bool> taskforce = Task.Factory.StartNew(() =>
                {
                    System.Threading.Thread.Sleep(4000);

                    bool implantadoSCCM = ExecutarComandoValidarLogado(comandosParam, chaveArquivo, idCookie, dominio, usuario, nomeArquivo, nomeComando);
                    return implantadoSCCM;
                });

                return ComumResultFactory.Criar(true, string.Format("Foi gerado uma implatação do SCCM - Arquivo/Comando: {0}/{1}", nomeArquivo, nomeComando));
            }
            else
            {
                bool implantadoSCCM = ExecutarComandoValidarLogado(comandosParam, chaveArquivo, idCookie, dominio, usuario, nomeArquivo, nomeComando);

                if (!implantadoSCCM)
                    return ComumResultFactory.Criar(implantadoSCCM, string.Format("Erro de implantação no SCCM - Arquivo/Comando: {0}/{1}", nomeArquivo, nomeComando));

                return ComumResultFactory.Criar(implantadoSCCM, string.Format("Comando: {0} implantado no SCCM", nomeComando), chaveArquivo);
            }
        }
        public bool ExecutarComandoValidarLogado(PSComandoParam[] comandos, string chaveArquivo, string idCookie, string dominio, string usuario, string nomeArquivo, string nomeComando)
        {
            IParseArquivo parseArquivo = new ParseArquivoXML(SCCM_SERVER);
            SMSCollection colecao = new SMSCollection();

            string script = string.Empty;
            string arquivo = ObterArquivoLocalValidarLogado(_arquivoCaminhoLocal, nomeArquivo, chaveArquivo, idCookie, dominio, usuario, comandos, parseArquivo);
            string psArquivo = string.Concat("'", arquivo, "'");

            string caminho = Path.Combine(ConfigurationManager.AppSettings.Get("PSDiretorioArquivos"), ObterArquivoNome(chaveArquivo, parseArquivo.Extensao));

            script = string.Concat(psArquivo, " | Out-File -Encoding \"UTF8\" ", caminho);

            bool implantadoSCCM = PSRequisicao.ExecutarResult(script);

            return implantadoSCCM;
        }
        public bool ExecutarComando(IParseArquivo parseArquivo, PSComandoParam[] comandos, string chaveArquivo, string nomeArquivo, string nomeComando)
        {
            string script = string.Empty;
            string arquivo = ObterArquivoLocal(_arquivoCaminhoLocal, nomeArquivo, chaveArquivo, comandos, parseArquivo);
            string psArquivo = string.Concat("'", arquivo, "'");

            string caminho = Path.Combine(ConfigurationManager.AppSettings.Get("PSDiretorioArquivos"), ObterArquivoNome(chaveArquivo, parseArquivo.Extensao));

            script = string.Concat(psArquivo, " | Out-File -Encoding \"UTF8\" ", caminho);

            bool result = PSRequisicao.ExecutarResult(script);

            return result;
        }
        public bool ExecutarScript(IParseArquivo parseArquivo, string dispositivo, string chave, string[] linhasComando)
        {
            string script = string.Empty;
            string arquivo = parseArquivo.Obter(chave, linhasComando);
            string psArquivo = string.Concat("'", arquivo, "'");

            string caminho = Path.Combine(ConfigurationManager.AppSettings.Get("PSDiretorioArquivos"), ObterArquivoNome(chave, parseArquivo.Extensao));

            script = string.Concat(psArquivo, " | Out-File -Encoding \"UTF8\" ", caminho);

            bool result = PSRequisicao.ExecutarResult(script);

            return result;
        }
        public bool ExecutarScript(string chave, string[] linhasComando)
        {
            IParseArquivo parseArquivo = new ParseArquivoXML(SCCM_SERVER);

            string script = string.Empty;
            string arquivo = parseArquivo.Obter(chave, linhasComando);
            string psArquivo = string.Concat("'", arquivo, "'");

            string caminho = Path.Combine(ConfigurationManager.AppSettings.Get("PSDiretorioArquivos"), ObterArquivoNome(chave, parseArquivo.Extensao));

            script = string.Concat(psArquivo, " | Out-File -Encoding \"UTF8\" ", caminho);

            bool result = PSRequisicao.ExecutarResult(script);

            return result;
        }
        public bool ExecutarScriptValidarLogado(string usuario, string dominio, string idCookie, string chave, string[] linhasScript)
        {
            string script = string.Empty;

            IParseArquivo parseArquivo = new ParseArquivoXML(SCCM_SERVER);

            string arquivo = ObterScriptValidarLogado(linhasScript, chave, idCookie, dominio, usuario, parseArquivo);
            string psArquivo = string.Concat("'", arquivo, "'");

            string caminho = Path.Combine(ConfigurationManager.AppSettings.Get("PSDiretorioArquivos"), ObterArquivoNome(chave, parseArquivo.Extensao));

            script = string.Concat(psArquivo, " | Out-File -Encoding \"UTF8\" ", caminho);

            bool result = PSRequisicao.ExecutarResult(script);

            return result;
        }
        public IComumResult ExecutarScriptResult(string[] linhasScript, string chaveArquivo)
        {
            bool implantadoSCCM = ExecutarScript(linhasScript, chaveArquivo);
            if (!implantadoSCCM)
                return ComumResultFactory.Criar(implantadoSCCM, string.Format("Erro de implantação no SCCM - ChaveArquivo: {0}", chaveArquivo));

            return ComumResultFactory.Criar(implantadoSCCM, string.Format("Script implantado no SCCM"), chaveArquivo);
        }
        public IComumResult ExecutarScriptValidarLogadoResult(string[] linhasScript, string dominio, string usuario, string chaveArquivo, string idCookie, bool gerarNovaThread = false)
        {
            if (gerarNovaThread)
            {
                Task<bool> taskforce = Task.Factory.StartNew(() =>
                {
                    System.Threading.Thread.Sleep(4000);

                    bool implantadoSCCM = ExecutarScriptValidarLogado(usuario, dominio, idCookie, chaveArquivo, linhasScript);
                    return implantadoSCCM;
                });

                return ComumResultFactory.Criar(true, string.Format("Script implantado no SCCM", chaveArquivo));
            }
            else
            {
                bool implantadoSCCM = ExecutarScriptValidarLogado(usuario, dominio, idCookie, chaveArquivo, linhasScript);
                if (!implantadoSCCM)
                    return ComumResultFactory.Criar(implantadoSCCM, string.Format("Erro de implantação no SCCM - ChaveArquivo: {0}", chaveArquivo));

                return ComumResultFactory.Criar(implantadoSCCM, string.Format("Script implantado no SCCM"), chaveArquivo);
            }
        }
        public bool ExecutarScript(string[] linhasComando, string chaveArquivo)
        {
            IParseArquivo parseArquivo = new ParseArquivoXML(SCCM_SERVER);

            string script = string.Empty;
            string arquivo = parseArquivo.Obter(chaveArquivo, linhasComando);
            string psArquivo = string.Concat("'", arquivo, "'");

            string caminho = Path.Combine(ConfigurationManager.AppSettings.Get("PSDiretorioArquivos"), ObterArquivoNome(chaveArquivo, parseArquivo.Extensao));

            script = string.Concat(psArquivo, " | Out-File -Encoding \"UTF8\" ", caminho);

            bool implantadoSCCM = PSRequisicao.ExecutarResult(script);

            return implantadoSCCM;
        }
        public bool ForcarClient(string triggerScheduleId, string device, bool xml = false)
        {
            string script = string.Empty;

            if (xml)
            {
                string xmlArquivo = string.Concat("$xml = [xml]'", ObterXMLPSTrigger(triggerScheduleId, device), "'\n");
                xmlArquivo = string.Concat(xmlArquivo, "$xml.save('", Path.Combine(ConfigurationManager.AppSettings.Get("PSDiretorioArquivos"), ObterArquivoNome(device)), "')");
                script = xmlArquivo;
            }
            else
            {
                script = ObterPSTrigger(triggerScheduleId, string.Empty);
            }

            bool result = PSRequisicao.ExecutarResult(script);

            return result;
        }

        #region Metodos Privados

        #region Antigo Servico Windows 
        private string ObterPSTrigger(string triggerScheduleId, string device)
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(device))
                sb.AppendLine(string.Format("$WMICaminho = \"\\\\{0}\\root\\ccm:SMS_Client\";", device));
            else
                sb.AppendLine("$WMICaminho = \"root\\ccm:SMS_Client\";");

            sb.AppendLine("$SMSWMI = [WmiClass]$WMICaminho;");
            sb.AppendLine(string.Format("$SMSWMI.TriggerSchedule(\"{0}\");", triggerScheduleId));

            return sb.ToString();
        }
        private string ObterArquivoNome(string device = null)
        {
            string dtProcessamento = DateTime.Now.ToString("yyyyMMddHHmmss");

            if (!string.IsNullOrEmpty(device))
                return string.Concat(PREF_ARQUIVO, device, "_", dtProcessamento, ".xml");

            return string.Concat(PREF_ARQUIVO, "Local_", dtProcessamento, ".xml");
        }
        private string ObterXMLPSTrigger(string triggerScheduleId, string device)
        {
            StringBuilder sbXML = new StringBuilder();
            MemoryStream stream = new MemoryStream();
            string result = string.Empty;

            using (XmlTextWriter xml = new XmlTextWriter(stream, Encoding.UTF8))
            {
                xml.WriteStartDocument();
                xml.IndentChar = '\t';
                xml.Formatting = System.Xml.Formatting.Indented;
                xml.WriteStartElement("smsclient");
                if (!string.IsNullOrEmpty(device))
                {
                    xml.WriteElementString("dispositivo", device);
                    xml.WriteElementString("script-1", string.Format("$WMICaminho = \"\\\\{0}\\root\\ccm:SMS_Client\";", device));
                }
                else
                {
                    xml.WriteElementString("dispositivo", "local");
                    xml.WriteElementString("script-1", "$WMICaminho = \"root\\ccm:SMS_Client\";");
                }
                xml.WriteElementString("script-2", "$SMSWMI = [WmiClass]$WMICaminho;");
                xml.WriteElementString("script-3", string.Format("$SMSWMI.TriggerSchedule(\"{0}\");", triggerScheduleId));
                xml.WriteEndElement();

                xml.Flush();

                StreamReader reader = new StreamReader(stream, Encoding.UTF8, true);
                stream.Seek(0, SeekOrigin.Begin);
                result = reader.ReadToEnd();

                return result;
            }
        }
        #endregion

        private string ObterArquivoNome(string chaveArquivo, string extensao)
        {
            string dtProcessamento = DateTime.Now.ToString("yyyyMMddHHmmss");
            extensao = extensao.Replace(".", "");

            return string.Concat(PREF_ARQUIVO, chaveArquivo, "_", dtProcessamento, ".", extensao.ToLower());
        }
        private bool GravarArquivoLocal(string[] linhas, string caminho, string nomeArquivo)
        {
            string arquivoCompleto = string.Empty;

            nomeArquivo = nomeArquivo.Split('.')[0];

            arquivoCompleto = Path.Combine(caminho, string.Concat(nomeArquivo.Trim(), ".txt"));
            string arquivoParam = string.Empty;

            bool contemFuncao = false;
            bool abriuFuncao = false;
            bool fechouFuncao = false;

            bool contemComando = false;

            bool contemParam = false;
            bool abriuParam = false;
            bool fechouParam = false;

            bool processouCabecalhoArquivo = false;

            StringBuilder sbArquivoParams = new StringBuilder();

            if (!Directory.Exists(caminho))
            {
                Directory.CreateDirectory(caminho);
            }
            if (File.Exists(arquivoCompleto))
            {
                throw new InvalidOperationException("Arquivo já existente", new Exception(string.Concat("O arquivo enviado já existe: ", nomeArquivo)));
            }

            try
            {
                using (FileStream stream = File.Create(arquivoCompleto))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        if (linhas == null || linhas.Length == 0)
                        {
                            writer.WriteLine("Não existem linhas!");
                        }
                        else
                        {
                            for (int indice = 0; indice < linhas.Count(); indice++)
                            {
                                try
                                {
                                    string linha = linhas[indice];

                                    if (string.IsNullOrWhiteSpace(linha))
                                        continue;

                                    string linhaUpper = linha.ToUpper();

                                    if ((linhas.Count() - 1) == indice)//Última linha - Fechamento do bloco }
                                    {
                                        if (linha.Contains(ARQUIVO_PARAM_FECHA_CHAVE))
                                        {
                                            int indiceFechaChave = linha.LastIndexOf(ARQUIVO_PARAM_FECHA_CHAVE);
                                            linha = linha.Remove(indiceFechaChave, (linha.Length - indiceFechaChave));
                                        }
                                    }

                                    if (!contemComando)
                                    {
                                        if (linhaUpper.Contains(ARQUIVO_COMANDO.ToUpper()))
                                        {
                                            contemComando = true;
                                        }

                                        ObterCabecalhoArquivoParams(linhaUpper, sbArquivoParams, ref contemFuncao, ref abriuFuncao, ref fechouFuncao, ref contemParam, ref abriuParam, ref fechouParam);
                                    }
                                    else if (!string.IsNullOrWhiteSpace(linha))
                                    {
                                        if (!processouCabecalhoArquivo)
                                        {
                                            if (!string.IsNullOrWhiteSpace(sbArquivoParams.ToString()))
                                            {
                                                writer.WriteLine(string.Concat(ARQUIVO_PARAM_GERADO.ToLower(), sbArquivoParams.ToString(), "\n"));
                                                processouCabecalhoArquivo = true;
                                            }
                                        }

                                        writer.WriteLine(linha);
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                            }

                            if (!contemComando)
                            {
                                throw new InvalidOperationException(string.Concat("Ocorreu um erro na geração do arquivo local: ", nomeArquivo), new Exception("O arquivo não possuí a tag '#SCCM_COMANDO', ou não possui comando."));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message.Contains("#SCCM_COMANDO"))
                {
                    if (File.Exists(arquivoCompleto))
                    {
                        File.Delete(arquivoCompleto);
                    }
                }
                throw new Exception(string.Concat("Ocorreu um erro na geração do arquivo local: ", nomeArquivo), ex.InnerException);
            }

            return true;
        }
        private string ObterArquivoLocal(string caminho, string nomeArquivo, string chave, PSComandoParam[] comandosParam, IParseArquivo parse)
        {
            string[] parametros = new string[] { };

            if (comandosParam == null)
            {
                throw new InvalidOperationException(string.Concat("Ocorreu um erro ao obter o comando no arquivo local.", nomeArquivo), new Exception("Obrigatório definir os comandos."));
            }

            comandosParam = comandosParam.OrderBy(valor => valor.Ordem)
                                         .ToArray();

            string arquivoCompleto = Path.Combine(caminho, string.Concat(nomeArquivo.Trim(), ".txt"));

            if (Directory.Exists(caminho))
            {
                if (File.Exists(arquivoCompleto))
                {
                    try
                    {
                        using (FileStream stream = File.Open(arquivoCompleto, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                string linha = string.Empty;

                                while ((linha = reader.ReadLine()) != null)
                                {
                                    string linhaUpper = linha.ToUpper();
                                    if (linhaUpper.Contains(ARQUIVO_PARAM_GERADO) && linhaUpper.Contains(ARQUIVO_PARAM_PS))//Contem Parâmetros iniciais
                                    {
                                        string comandos = linhaUpper.Replace(ARQUIVO_PARAM_GERADO, "");
                                        parametros = comandos.Split('|');

                                        parametros.ToList().ForEach(param =>
                                        {
                                            if (!string.IsNullOrWhiteSpace(param))
                                            {
                                                bool valido = comandosParam.ToList().Exists(comandoParam => string.CompareOrdinal(comandoParam.Nome.ToUpper(), param.ToUpper()) == 0
                                                                                                         || string.CompareOrdinal(comandoParam.Nome.ToUpper(), param.Replace(ARQUIVO_PARAM_PS, "").ToUpper()) == 0);
                                                if (!valido)
                                                {
                                                    throw new InvalidOperationException(string.Concat("Parêmtro(s) não inválido(s): ", nomeArquivo), new Exception("Os parâmetros não são válidos no script"));
                                                }
                                            }
                                        });
                                    }

                                    comandosParam.ToList().ForEach(comandoParam => //Linha contem parâmetros, realiza o replace
                                    {
                                        if (linhaUpper.Contains(comandoParam.Nome) && !linhaUpper.Contains(ARQUIVO_PARAM_GERADO))//Não contem parâmetros iniciais
                                        {
                                            string comando = comandoParam.Nome.Contains(ARQUIVO_PARAM_PS) ? comandoParam.Nome : string.Concat("$", comandoParam.Nome);

                                            linhaUpper = linhaUpper.Replace(comando, comandoParam.Valor);
                                            linha = linhaUpper.Replace(comando, comandoParam.Valor);
                                        }
                                    });

                                    if (!linhaUpper.Contains(ARQUIVO_PARAM_GERADO) && !string.IsNullOrWhiteSpace(linhaUpper))//Não contem parâmetros iniciais e a lnha não é vazio
                                    {
                                        parse.Definir(linha);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Concat("Ocorreu um erro ao obter o comando no arquivo local: ", nomeArquivo), ex.InnerException);
                    }
                }
            }

            return parse.Obter(chave);
        }
        private string ObterScriptValidarLogado(string[] linhasScript, string chave, string idCookie, string dominio, string usuario, IParseArquivo parse)
        {
            string abreComando = "[ABRE-COMANDO]";
            string fechaComando = "[FECHA-COMANDO]";

            bool abriuComando = false;

            string dispositivo = "SERVER";

            SMSRSystem smsDisp = new SMSRSystem();
            StringBuilder sbComando = new StringBuilder();

            linhasScript.ToList().ForEach(linha =>
            {
                sbComando.AppendLine(linha);
            });

            string[] scriptLinhas = smsDisp.ScriptValidarCookie(sbComando.ToString(), dominio, usuario, idCookie, "Disp");

            try
            {
                foreach (string sctLinha in scriptLinhas)
                {
                    string linha = sctLinha;
                    string linhaUpper = linha.ToUpper();

                    if (linhaUpper.Contains(abreComando) && !abriuComando)
                    {
                        abriuComando = true;
                    }
                    if (linhaUpper.Contains(fechaComando) && abriuComando)
                    {
                        abriuComando = false;
                    }

                    if (abriuComando)
                    {
                        if (!linhaUpper.Contains(ARQUIVO_PARAM_GERADO) && !string.IsNullOrWhiteSpace(linhaUpper))//Não contém parâmetros iniciais e a linha não é vazio
                        {
                            parse.Definir(linha);
                        }
                    }
                }

                return parse.Obter(chave);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro na geração do comando validando usuário logado", ex.InnerException);
            }
        }
        private string ObterArquivoLocalValidarLogado(string caminho, string nomeArquivo, string chave, string idCookie, string dominio, string usuario, PSComandoParam[] comandosParam, IParseArquivo parse)
        {
            string abreComando = "[ABRE-COMANDO]";
            string fechaComando = "[FECHA-COMANDO]";

            bool abriuComando = false;

            string[] parametros = new string[] { };

            if (comandosParam == null)
            {
                throw new InvalidOperationException(string.Concat("Ocorreu um erro ao obter o comando no arquivo local.", nomeArquivo), new Exception("Obrigatório definir os comandos."));
            }

            comandosParam = comandosParam.OrderBy(valor => valor.Ordem)
                                         .ToArray();

            string arquivoCompleto = Path.Combine(caminho, string.Concat(nomeArquivo.Trim(), ".txt"));

            if (Directory.Exists(caminho))
            {
                if (File.Exists(arquivoCompleto))
                {
                    string scriptComando = File.ReadAllText(arquivoCompleto);
                    string[] scriptLinhas = new string[] { };

                    SMSRSystem smsDisp = new SMSRSystem();

                    if (comandosParam != null && comandosParam.Count() > 0)
                    {
                        PSComandoParam psComando = comandosParam.FirstOrDefault();
                        bool filtroHost = string.IsNullOrWhiteSpace(psComando.Valor);

                        if (filtroHost && comandosParam.Count() == 1)
                        {
                            if (psComando.Nome.ToUpper().Contains("HOST") || psComando.Nome.ToUpper().Contains("COMP") || psComando.Nome.ToUpper().Contains("DISP"))
                            {
                                scriptLinhas = smsDisp.ScriptValidarCookie(scriptComando, dominio, usuario, idCookie, psComando.Nome);
                            }
                        }
                        else
                        {
                            scriptLinhas = smsDisp.ScriptValidarCookie(scriptComando, dominio, usuario, idCookie);
                        }
                    }
                    try
                    {
                        foreach (string sctLinha in scriptLinhas)
                        {
                            string linha = sctLinha;
                            string linhaUpper = linha.ToUpper();

                            if (linhaUpper.Contains(abreComando) && !abriuComando)
                            {
                                abriuComando = true;
                            }
                            if (linhaUpper.Contains(fechaComando) && abriuComando)
                            {
                                abriuComando = false;
                            }

                            if (abriuComando)
                            {
                                if (linhaUpper.Contains(ARQUIVO_PARAM_GERADO) && linhaUpper.Contains(ARQUIVO_PARAM_PS))//Contem Parâmetros iniciais
                                {
                                    string comandos = linhaUpper.Replace(ARQUIVO_PARAM_GERADO, "");
                                    parametros = comandos.Split('|');

                                    parametros.ToList().ForEach(param =>
                                    {
                                        if (!string.IsNullOrWhiteSpace(param))
                                        {
                                            foreach (PSComandoParam comandoPa in comandosParam.ToList())
                                            {
                                                if (string.IsNullOrWhiteSpace(comandoPa.Valor))
                                                    continue;

                                                bool valido = comandosParam.ToList().Exists(comandoParam => string.CompareOrdinal(comandoParam.Nome.Trim().ToUpper(), param.Trim().ToUpper()) == 0
                                                                                                         || string.CompareOrdinal(comandoParam.Nome.Trim().ToUpper(), param.Trim().Replace(ARQUIVO_PARAM_PS, "").ToUpper()) == 0);
                                                if (!valido)
                                                {
                                                    throw new InvalidOperationException(string.Concat("Parêmtro(s) não inválido(s): ", nomeArquivo), new Exception("Os parâmetros não são válidos no script"));
                                                }
                                            }
                                        }
                                    });
                                }
                                comandosParam.ToList().ForEach(comandoParam => //Linha contem parâmetros, realiza o replace
                                {
                                    if (linhaUpper.Contains(comandoParam.Nome) && !linhaUpper.Contains(ARQUIVO_PARAM_GERADO))//Não contem parâmetros iniciais
                                    {
                                        string comando = comandoParam.Nome.Contains(ARQUIVO_PARAM_PS) ? comandoParam.Nome : string.Concat("$", comandoParam.Nome);

                                        linhaUpper = linhaUpper.Replace(comando, comandoParam.Valor);
                                        linha = linhaUpper.Replace(comando, comandoParam.Valor);
                                    }
                                });
                            }

                            if (!linhaUpper.Contains(ARQUIVO_PARAM_GERADO) && !string.IsNullOrWhiteSpace(linhaUpper))//Não contem parâmetros iniciais e a lnha não é vazio
                            {
                                parse.Definir(linha);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Concat("Ocorreu um erro ao obter o comando no arquivo local: ", nomeArquivo), ex.InnerException);
                    }
                }
            }

            return parse.Obter(chave);
        }
        private List<string> ObterArquivoLocal(string caminho, string nomeArquivo)
        {
            List<string> lstLinhasComando = new List<string>();

            string arquivoCompleto = Path.Combine(caminho, string.Concat(nomeArquivo, ".txt"));

            if (Directory.Exists(caminho))
            {
                if (File.Exists(arquivoCompleto))
                {
                    try
                    {
                        using (FileStream stream = File.Open(caminho, FileMode.Open))
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                string linha = reader.ReadLine();

                                lstLinhasComando.Add(linha);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Concat("Ocorreu um erro ao obter o comando no arquivo local: ", nomeArquivo), ex.InnerException);
                    }
                }
            }

            return lstLinhasComando;
        }
        private string[] ObterArquivoLocalJSONResult(string caminho, string nomeArquivo, IParseArquivoResult parseResult)
        {
            string json = string.Empty;

            DirectoryInfo diretorio = new DirectoryInfo(caminho);
            FileInfo[] arrayArquivo = diretorio.GetFiles();

            List<string> lstJsonResult = new List<string>();

            foreach (FileInfo arquivo in arrayArquivo)
            {
                if (arquivo.Name.Contains(nomeArquivo))
                {
                    string resultTexto = File.ReadAllText(arquivo.FullName);

                    if (parseResult.Valido(resultTexto))
                    {
                        json = parseResult.ParseResult(resultTexto);
                    }
                    else
                    {
                        json = JsonConvert.SerializeObject(new { result = string.Concat("Não foi possível realizar o parse do resultado - Extensao: .", arquivo.Extension) });
                    }

                    lstJsonResult.Add(json);
                }
            }

            return lstJsonResult.ToArray();
        }
        private bool RemoverArquivoLocal(string caminho, string nomeArquivo)
        {
            string arquivoCompleto = Path.Combine(caminho, string.Concat(nomeArquivo, ".txt"));

            if (Directory.Exists(caminho))
            {
                if (File.Exists(arquivoCompleto))
                {
                    File.Delete(arquivoCompleto);

                    return true;
                }
            }

            return false;
        }
        private void ObterCabecalhoArquivoParams(string linhaUpper, StringBuilder sbArquivoParams, ref bool contemFuncao, ref bool abriuFuncao, ref bool fechouFuncao, ref bool contemParam, ref bool abriuParam, ref bool fechouParam)
        {
            if (contemFuncao || linhaUpper.Contains(ARQUIVO_FUNCTION.ToUpper()))
            {
                contemFuncao = true;

                if (!fechouFuncao)
                {
                    if (linhaUpper.Contains(ARQUIVO_FUNCTION_ABRE))
                    {
                        abriuFuncao = true;

                        ObterParamArquivo(linhaUpper, sbArquivoParams);
                    }
                }
                if (abriuFuncao)
                {
                    if (linhaUpper.Contains(ARQUIVO_FUNCTION_FECHA))
                    {
                        fechouFuncao = true;
                    }

                    ObterParamArquivo(linhaUpper, sbArquivoParams);
                }

                if (abriuFuncao && fechouFuncao)
                {
                    contemFuncao = false;
                }
            }

            if (contemParam || linhaUpper.Contains(ARQUIVO_PARAM.ToUpper()))
            {
                contemParam = true;
                contemFuncao = false;

                if (linhaUpper.Contains(ARQUIVO_COMUM_AF))
                {
                    linhaUpper = linhaUpper.Replace(ARQUIVO_COMUM_AF, "");
                }

                if (linhaUpper.Contains(ARQUIVO_PARAM_ABRE) && !(linhaUpper.Contains(ARQUIVO_PARAM_ABRE_COLCHETE) || linhaUpper.Contains(ARQUIVO_PARAM_FECHA_COLCHETE)))
                {
                    if (linhaUpper.Contains(ARQUIVO_PARAM_ABRE))
                    {
                        abriuParam = true;
                    }

                    ObterParamArquivo(linhaUpper, sbArquivoParams);
                }
                if (abriuParam)
                {
                    if (linhaUpper.Contains(ARQUIVO_FUNCTION_FECHA) && !(linhaUpper.Contains(ARQUIVO_PARAM_ABRE_COLCHETE) || linhaUpper.Contains(ARQUIVO_PARAM_FECHA_COLCHETE)))
                    {
                        fechouParam = true;
                    }

                    ObterParamArquivo(linhaUpper, sbArquivoParams);
                }

                if (abriuParam && fechouParam)
                {
                    contemParam = false;
                }
            }
        }
        private void ObterParamArquivo(string linha, StringBuilder sbArquivoParams)
        {
            try
            {
                int indiceParamInicio = 0;

                string boolParamTrue = "$true";
                string boolParamFalse = "$false";

                string param = string.Empty;

                if (linha.Contains(ARQUIVO_PARAM_SEPARACAO_PS))
                {
                    string[] linhasParam = linha.Split(',');

                    linhasParam.ToList().ForEach(linhaParam =>
                    {
                        if (linhaParam.Contains(ARQUIVO_PARAM_PS))
                        {
                            linhaParam = linhaParam.Trim();

                            indiceParamInicio = 0;
                            indiceParamInicio = linhaParam.IndexOf(ARQUIVO_PARAM_PS);
                            param = linhaParam.Substring(indiceParamInicio, (linhaParam.Length - indiceParamInicio));

                            string[] linhaTipo = linha.Split(new char[] { ' ' });

                            if (linhaTipo != null)
                            {
                                string linhaFinal = linhaTipo.Where(valor => valor.Contains(param))
                                                             .FirstOrDefault();

                                if (linhaFinal != null)
                                {
                                    indiceParamInicio = linhaFinal.IndexOf(ARQUIVO_PARAM_PS);
                                    param = linhaFinal.Substring(indiceParamInicio, (linhaFinal.Length - indiceParamInicio));
                                    param = param.Replace(ARQUIVO_PARAM_SEPARACAO_PS, "").Replace(ARQUIVO_PARAM_FECHA, "").Replace(ARQUIVO_PARAM_ABRE_CHAVE, "").Trim();
                                }
                            }

                            param = param.Replace(ARQUIVO_PARAM_ABRE, "").Replace(ARQUIVO_PARAM_SEPARACAO_PS, "").Replace(ARQUIVO_PARAM_FECHA, "").Replace(ARQUIVO_PARAM_ABRE_CHAVE, "").Replace(ARQUIVO_PARAM_ABRE_COLCHETE, "").Replace(ARQUIVO_PARAM_FECHA_COLCHETE, "").Trim();

                            if (param.Contains("="))
                            {
                                int inidiceIgualdade = param.IndexOf("=");
                                param = param.Remove(inidiceIgualdade, (inidiceIgualdade - param.Length));
                            }

                            if (!sbArquivoParams.ToString().Contains(param) && (!(param.ToUpper().Contains(boolParamTrue.ToUpper()) || param.Contains(boolParamFalse.ToUpper()))))
                                sbArquivoParams.Append(string.Concat(param, "|"));
                        }
                    });
                }
                else
                {
                    if (linha.Contains(ARQUIVO_PARAM_PS))
                    {
                        linha = linha.Trim();
                        indiceParamInicio = linha.IndexOf(ARQUIVO_PARAM_PS);
                        param = linha.Substring(indiceParamInicio, (linha.Length - indiceParamInicio));
                        param = param.Replace(ARQUIVO_PARAM_SEPARACAO_PS, "").Replace(ARQUIVO_PARAM_FECHA, "").Replace(ARQUIVO_PARAM_ABRE_CHAVE, "").Trim();

                        string[] linhaTipo = linha.Split(new char[] { ' ' });

                        if (linhaTipo != null)
                        {
                            string linhaFinal = linhaTipo.Where(valor => valor.Contains(param))
                                                         .FirstOrDefault();

                            if (linhaFinal != null)
                            {
                                indiceParamInicio = linhaFinal.IndexOf(ARQUIVO_PARAM_PS);
                                param = linhaFinal.Substring(indiceParamInicio, (linhaFinal.Length - indiceParamInicio));
                                param = param.Replace(ARQUIVO_PARAM_SEPARACAO_PS, "").Replace(ARQUIVO_PARAM_FECHA, "").Replace(ARQUIVO_PARAM_ABRE_CHAVE, "").Trim();
                            }
                        }

                        param = param.Replace(ARQUIVO_PARAM_ABRE, "").Replace(ARQUIVO_PARAM_SEPARACAO_PS, "").Replace(ARQUIVO_PARAM_FECHA, "").Replace(ARQUIVO_PARAM_ABRE_CHAVE, "").Replace(ARQUIVO_PARAM_ABRE_COLCHETE, "").Replace(ARQUIVO_PARAM_FECHA_COLCHETE, "").Trim();

                        if (param.Contains("="))
                        {
                            int inidiceIgualdade = param.IndexOf("=");
                            param = param.Remove(inidiceIgualdade, (param.Length - inidiceIgualdade));
                        }

                        if (!sbArquivoParams.ToString().Contains(param) && (!(param.ToUpper().Contains(boolParamTrue.ToUpper()) || param.Contains(boolParamFalse.ToUpper()))))
                            sbArquivoParams.Append(string.Concat(param, "|"));
                    }
                }
            }
            catch
            {
                throw new Exception("Geração do arquivo", new Exception("Ocorre um erro ao obter os parâmetros do script"));
            }
        }

        #endregion
    }
}
