using MyBranch.Log.Dominio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBranch.Log.Factory
{
    public class LogLocal : IMyBranchLog
    {
        private string _nomeArquivoDef;
        private const string LOG_DESC = "EComigo_Log";
        public void NomeArquivoLogDefault()
        {
            _nomeArquivoDef = LOG_DESC;
        }
        public void NomeArquivoLog(string nome)
        {
            DefinirNomeLog(nome);
        }
        public void DefaultNomeArquivoLog()
        {
            throw new NotImplementedException();
        }
        public bool GravarLog(object objeto)
        {
            throw new NotImplementedException();
        }

        public bool GravarLog(string msgLog)
        {
            Gerar(msgLog);
            return true;
        }

        public bool GravarLog(object objeto, Exception exception)
        {
            throw new NotImplementedException();
        }

        public bool GravarLog(string msgLog, Exception exception)
        {
            Gerar(msgLog, exception);
            return true;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #region Metodos Privados
        private void DefinirNomeLog(string nomeArquivo = null)
        {
            try
            {
                string currentDir = System.AppDomain.CurrentDomain.BaseDirectory;

                if (string.IsNullOrWhiteSpace(nomeArquivo))
                {
                    _nomeArquivoDef = nomeArquivo;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro na biblioteca de lo Log4Net", ex.InnerException);
            }
        }
        private void Gerar(string mensagem, Exception exception = null)
        {
            try
            {
                string arquivo = string.Empty;
                string caminhoArq = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "Exception");

                string _dataProcessamento = DateTime.Now.ToString("yyyyMMddHHmmss");

                if (string.IsNullOrEmpty(caminhoArq))
                {
                    throw new InvalidOperationException("Ocorreu um erro na geração do Log.", new Exception("Caminho inválido."));
                }

                arquivo = Path.Combine(caminhoArq, string.Concat("LOGDefault-", "_", _dataProcessamento, ".txt"));

                if (!Directory.Exists(caminhoArq))
                {
                    Directory.CreateDirectory(caminhoArq);
                }
                if (File.Exists(arquivo))
                {
                    File.Delete(arquivo);
                }

                if (exception != null)
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(mensagem);
                    sb.AppendLine(exception.Message);
                    sb.AppendLine((exception.InnerException != null) ? exception.InnerException.Message : null);
                    sb.AppendLine(exception.StackTrace);
                    sb.AppendLine((exception.InnerException != null) ? exception.InnerException.StackTrace : null);

                    mensagem = sb.ToString();
                }

                using (FileStream fs = File.Create(arquivo))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        Escrever(mensagem, writer);
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                GravarLog(string.Concat(ex.Message, " - ", ex.InnerException));
            }
            catch (Exception ex)
            {
                GravarLog(string.Concat("Ocorreu um erro na geração do Log - ", ex.InnerException));
            }
        }
        private void Escrever(string mensagem, TextWriter txtWriter)
        {
            try
            {
                txtWriter.WriteLine("{0}", mensagem);
                txtWriter.WriteLine("-------------------------------------------------------------------------");
            }
            catch { }
        }
        #endregion
    }
}
