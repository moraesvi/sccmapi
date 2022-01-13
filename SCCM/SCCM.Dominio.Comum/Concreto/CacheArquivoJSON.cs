using Newtonsoft.Json;
using System;
using HelperComum;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SCCM.Dominio.Comum.Concreto
{
    public class CacheArquivoJSON : ICache
    {
        private string _extensao;
        private string _currentDir;
        private string _caminho;
        private int _tempoCache;
        private bool _cacheDefinido;

        private const string EXTENSAO = "json";
        private const int ERROR_SHARING_VIOLATION = 32;
        private const int ERROR_LOCK_VIOLATION = 33;

        public CacheArquivoJSON()
        {
            _extensao = EXTENSAO;
            _currentDir = System.AppDomain.CurrentDomain.BaseDirectory;
            _caminho = Path.Combine(_currentDir, "Cache");
            _tempoCache = -1;
            _cacheDefinido = false;
        }
        public CacheArquivoJSON(int tempoCacheMinutos)
        {
            _extensao = EXTENSAO;
            _currentDir = System.AppDomain.CurrentDomain.BaseDirectory;
            _caminho = Path.Combine(_currentDir, "Cache");
            _tempoCache = tempoCacheMinutos;
            _cacheDefinido = true;
        }
        public bool Definir(string dados, string chave)
        {
            try
            {
                string arquivoCompleto = DefinirNomeArquivo(chave, _tempoCache);

                if (!string.IsNullOrWhiteSpace(dados))
                {
                    return false;
                }
                if (!Directory.Exists(_caminho))
                {
                    Directory.CreateDirectory(_caminho);
                }
                if (File.Exists(arquivoCompleto))
                {
                    File.Delete(arquivoCompleto);
                }

                try
                {
                    using (FileStream stream = File.Create(arquivoCompleto))
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.WriteLine(dados);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Concat("Ocorreu um erro na geração do arquivo de cache ", chave), ex.InnerException);
                }

                return true;
            }
            catch (IOException ex)
            {
                if (ArquivoCacheAberto(ex))
                {
                    return false;
                }

                throw new Exception(string.Concat("Ocorreu um erro na geração do arquivo de cache ", chave), ex.InnerException);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Concat("Ocorreu um erro na geração do arquivo de cache ", chave), ex.InnerException);
            }
        }
        public string Obter(string chave)
        {
            try
            {
                if (Directory.Exists(_caminho))
                {
                    string arquivoCompleto = Path.Combine(_caminho, string.Concat(chave.Trim(), _extensao));
                    if (File.Exists(arquivoCompleto))
                    {
                        string json = File.ReadAllText(arquivoCompleto);

                        return json;
                    }
                }

                return null;
            }
            catch (IOException ex)
            {
                if (ArquivoCacheAberto(ex))
                {
                    return string.Empty;
                }

                throw new Exception(string.Concat("Ocorreu um erro na geração do arquivo de cache ", chave), ex.InnerException);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Concat("Ocorreu um erro ao obter o cache: ", chave), ex.InnerException);
            }
        }
        public T[] ObterArray<T>(string chave) where T : class
        {
            try
            {
                T[] arrayJson = null;

                if (Directory.Exists(_caminho))
                {

                    if (!_cacheDefinido)
                    {
                        string arquivoCompleto = Path.Combine(_caminho, string.Concat(chave.Trim(), _extensao));

                        arrayJson = ObterArquivoJSONCache<T>(arquivoCompleto);
                    }
                    else
                    {
                        DateTime dtNow = DateTime.Now;
                        FileInfo[] arrayArquivo = ObterArrayArquivosJSON(_caminho);

                        foreach (FileInfo arquivo in arrayArquivo)
                        {
                            string nomeArquivo = arquivo.Name;
                            int indiceDtProcArquivo = nomeArquivo.IndexOf("-DT_");

                            string nomeArqDtProcessamento = nomeArquivo.Substring(indiceDtProcArquivo, (nomeArquivo.Length - indiceDtProcArquivo));

                            DateTime dtProc = DateTime.Parse(nomeArqDtProcessamento);

                            double totalDifMinutos = (dtNow - dtProc).TotalMinutes;

                            if (totalDifMinutos > _tempoCache) //DataArquivoSalvo - DataAgora > Tempo do cache
                            {
                                string arquivoCompleto = Path.Combine(_caminho, string.Concat(chave.Trim(), _extensao));

                                arrayJson = ObterArquivoJSONCache<T>(arquivoCompleto);
                            }
                        }
                    }
                }

                return arrayJson;
            }
            catch (IOException ex)
            {
                if (ArquivoCacheAberto(ex))
                {
                    return null;
                }

                throw new Exception(string.Concat("Ocorreu um erro na geração do arquivo de cache ", chave), ex.InnerException);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Concat("Ocorreu um erro ao obter o cache: ", chave), ex.InnerException);
            }
        }
        public T Obter<T>(string chave) where T : class
        {
            try
            {
                if (Directory.Exists(_caminho))
                {
                    if (!_cacheDefinido)
                    {
                        string arquivoCompleto = Path.Combine(_caminho, string.Concat(chave.Trim(), _extensao));
                        if (File.Exists(arquivoCompleto))
                        {
                            string json = File.ReadAllText(arquivoCompleto);

                            T objeto = JsonConvert.DeserializeObject<T>(json);

                            return objeto;
                        }
                    }
                    else
                    {
                        DateTime dtNow = DateTime.Now;
                        FileInfo[] arrayArquivo = ObterArrayArquivosJSON(_caminho);

                        foreach (FileInfo arquivo in arrayArquivo)
                        {
                            string nomeArquivo = arquivo.Name;

                            if (nomeArquivo.Contains(chave))
                            {
                                double totalDifMinutos = (dtNow - arquivo.LastWriteTime).TotalMinutes;

                                if (Math.Round(totalDifMinutos) < _tempoCache) //(DataArquivoSalvo - DataAgora) > Tempo do cache
                                {
                                    string arquivoCompleto = Path.Combine(_caminho, string.Concat(chave.Trim(), ".", _extensao));

                                    string json = File.ReadAllText(arquivoCompleto);

                                    T objeto = JsonConvert.DeserializeObject<T>(json);

                                    return objeto;
                                }
                            }
                        }
                    }
                }

                return null;
            }
            catch (IOException ex)
            {
                if (ArquivoCacheAberto(ex))
                {
                    return null;
                }

                throw new Exception(string.Concat("Ocorreu um erro na geração do arquivo de cache ", chave), ex.InnerException);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Concat("Ocorreu um erro ao obter o cache: ", chave), ex.InnerException);
            }
        }

        #region Metodos Privados

        private string DefinirNomeArquivo(string chave, int tempoCache)
        {
            string nomeArquivo = string.Empty;

            if (!_cacheDefinido)
            {
                nomeArquivo = Path.Combine(_caminho, string.Concat(chave.Trim(), _extensao));
            }
            else
            {
                nomeArquivo = Path.Combine(_caminho, string.Concat(chave.Trim(), ".", _extensao));
            }

            return nomeArquivo;
        }
        private bool ArquivoCacheAberto(Exception exception)
        {
            int errorCode = Marshal.GetHRForException(exception) & ((1 << 16) - 1);
            return errorCode == ERROR_SHARING_VIOLATION || errorCode == ERROR_LOCK_VIOLATION;
        }
        private T[] ObterArquivoJSONCache<T>(string caminhoArquivoCompleto) where T : class
        {
            if (File.Exists(caminhoArquivoCompleto))
            {
                string json = File.ReadAllText(caminhoArquivoCompleto);

                //Necessário caso o model não possua construtor sem parâmetros.
                JArray jsonArray = JArray.Parse(json);

                List<T> lstResult = new List<T>();

                foreach (var js in jsonArray)
                {
                    T objetoResult = Helper.CriarInstancia(typeof(T)) as T;

                    JsonConvert.PopulateObject(JsonConvert.SerializeObject(js), objetoResult);
                    lstResult.Add(objetoResult);
                }

                return lstResult.ToArray();
            }

            return null;
        }
        private FileInfo[] ObterArrayArquivosJSON(string diretorio, bool gerarException = false, string exception = null, string innerException = null)
        {
            DirectoryInfo diretorioJsonParse = new DirectoryInfo(diretorio);
            FileInfo[] arrayJson = diretorioJsonParse.GetFiles("*.json");

            if (arrayJson == null || arrayJson.Length == 0)
            {
                if (gerarException)
                {
                    throw new InvalidOperationException(exception, new Exception(innerException));
                }
            }

            return arrayJson;
        }

        #endregion
    }
}
