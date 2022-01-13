using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HelperComum
{
    public class Log
    {
        private string _sistema = string.Empty;

        private string _dataProcessamento;

        public Log(string sistema)
        {
            _sistema = sistema;
        }
        public void Novo(string mensagem, string descArquivo = null)
        {
            try
            {
                string arquivo = string.Empty;
                string caminhoArq = Path.GetDirectoryName(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log\\"));

                _dataProcessamento = DateTime.Now.ToString("yyyyMMddHHmmss");

                if (string.IsNullOrEmpty(caminhoArq))
                {
                    throw new InvalidOperationException("Ocorreu um erro na geração do Log.", new Exception("Caminho inválido."));
                }

                if (!string.IsNullOrEmpty(descArquivo))
                {
                    arquivo = Path.Combine(caminhoArq, string.Concat("log-", _sistema, "_", descArquivo, "_", _dataProcessamento, ".txt"));
                }
                else
                {
                    arquivo = Path.Combine(caminhoArq, string.Concat("log-", _sistema, "_", _dataProcessamento, ".txt"));
                }

                if (!Directory.Exists(caminhoArq))
                {
                    Directory.CreateDirectory(caminhoArq);
                }
                if (File.Exists(arquivo))
                {
                    File.Delete(arquivo);
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
                Novo(string.Concat(ex.Message, " - ", ex.InnerException));
            }
            catch (Exception ex)
            {
                Novo(string.Concat("Ocorreu um erro na geração do Log - ", ex.InnerException));
            }
        }
        private void Escrever(string mensagem, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write("\r\nLog - {0} : ", _sistema.ToUpper());
                txtWriter.WriteLine("{0}", DateTime.Now.ToLongDateString());
                txtWriter.WriteLine(":");
                txtWriter.WriteLine(":{0}", mensagem);
                txtWriter.WriteLine("-------------------------------");
            }
            catch
            {
            }
        }
    }
}
