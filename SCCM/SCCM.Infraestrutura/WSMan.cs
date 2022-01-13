using HelperComum;
using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Infraestrutura
{
    static class WSMan
    {
        private const string PS_PUT_TEMPLATE = "${0}.put()";
        private const string PS_DELETE_QUERY_RESULT = @"$deletado = $false;
                                                        $totalDel = 0;

                                                        Foreach ($query in $WmiObject) {
                                                        	$deletado = $true;
                                                            $totalDel++
                                                        	$query.Delete()
                                                        }
                                                        
                                                        $Prop = @{
                                                        	Total = $totalDel
	                                                        TotalDesc = ""$($totalDel) registro(s) excluído(s)""
                                                        	DataProc = (Get-Date).ToString()
                                                            Excluiu = $deletado
                                                        }
                                                        
                                                        New-Object -TypeName PSObject -prop $Prop";
        private const string PS_CMD_EXITSTE_TEMPLATE = @"$propResult =  
                                                          @{
                                                             Existe = $false;
                                                          }
                                                          if (Get-Command -Name ""[cmd-nome]"" -errorAction SilentlyContinue)
                                                          {
                                                          	 $propResult.Existe = true
                                                          }
                                                          New-Object -TypeName PSObject -prop $propResult;";

        internal static void ConexaoRunspace(WSManConnectionInfo connectionInfo, ref Runspace remoteRunspace)
        {
            remoteRunspace = RunspaceFactory.CreateRunspace(connectionInfo);
            remoteRunspace.Open();
        }

        internal static string ExecutarPSScriptAsString(string script, Runspace remoteRunspace)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();

                ICollection<PSObject> results = ExecutarPSScript(script, remoteRunspace);

                foreach (PSObject obj in results)
                {
                    try
                    {
                        if (obj != null)
                        {
                            stringBuilder.AppendLine(obj.ToString());
                        }
                    }
                    catch { }
                }


                return stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                string innerErro = PowerShellExceptionFormat(ex, script);

                throw new Exception("Ocorreu um erro de exeução de script PowerShell", new Exception(innerErro));
            }
        }
        internal static bool ExecutarResultPSScript(string script, Runspace remoteRunspace)
        {

            try
            {
                using (PowerShell powershell = PowerShell.Create())
                {
                    powershell.Runspace = remoteRunspace;
                    powershell.AddScript(script);

                    PSObject psObject = powershell.Invoke()
                                                  .FirstOrDefault();

                    if (psObject == null)
                    {
                        PowerShellErrosExecucao(powershell, script);
                    }

                    return true;
                }
            }
            catch (InvalidPowerShellStateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                string innerErro = PowerShellExceptionFormat(ex, script);

                throw new Exception("Ocorreu um erro de exeução de script PowerShell", new Exception(innerErro));
            }
        }
        internal static T ExecutarPSScript<T>(string queryCompleto, T objeto, Runspace remoteRunspace) where T : class
        {

            try
            {
                using (PowerShell powershell = PowerShell.Create())
                {
                    powershell.Runspace = remoteRunspace;
                    powershell.AddScript(queryCompleto);

                    PSObject psObject = powershell.Invoke()
                                                  .FirstOrDefault();

                    SCCMHelper.PSObjectObjeto(psObject, objeto);

                    if (psObject == null)
                    {
                        PowerShellErrosExecucao(powershell, queryCompleto);
                    }

                    return objeto;
                }
            }
            catch (InvalidPowerShellStateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                string innerErro = PowerShellExceptionFormat(ex, queryCompleto, objeto);

                throw new Exception("Ocorreu um erro de exeução de script PowerShell", new Exception(innerErro));
            }
        }
        internal static T ExecutarPSScript<T>(string script, string nomeParam, object param, T objeto, Runspace remoteRunspace) where T : class
        {

            try
            {
                using (PowerShell powershell = PowerShell.Create())
                {
                    powershell.Runspace = remoteRunspace;
                    powershell.AddScript(script);
                    powershell.AddParameter(nomeParam, param);

                    PSObject psObject = powershell.Invoke()
                                                  .FirstOrDefault();

                    if (psObject == null)
                    {
                        PowerShellErrosExecucao(powershell, script);
                    }

                    SCCMHelper.PSObjectObjeto(psObject, objeto);

                    return objeto;
                }
            }
            catch (InvalidPowerShellStateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                string innerErro = PowerShellExceptionFormat(ex, script, objeto);

                throw new Exception("Ocorreu um erro de exeução de script PowerShell", new Exception(innerErro));
            }
        }
        internal static ICollection<T> ExecutarPSScriptColecao<T>(string script, string nomeParam, object param, T objeto, Runspace remoteRunspace) where T : class
        {
            try
            {
                using (PowerShell powershell = PowerShell.Create())
                {
                    powershell.Runspace = remoteRunspace;
                    powershell.AddScript(script);
                    powershell.AddParameter(nomeParam, param);

                    ICollection<PSObject> psResults = powershell.Invoke();

                    ICollection<T> results = new List<T>();

                    if (psResults == null || psResults.Count == 0)
                    {
                        PowerShellErrosExecucao(powershell, script);
                    }

                    foreach (PSObject po in psResults)
                    {
                        if (po != null)
                        {
                            objeto = Helper.CriarInstancia(objeto.GetType()) as T;
                            SCCMHelper.PSObjectObjeto(po, objeto);
                            results.Add(objeto);
                        }
                    }

                    return results;
                }
            }
            catch (InvalidPowerShellStateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                StringBuilder sbInnerErro = new StringBuilder();

                string innerErro = PowerShellExceptionFormat(ex, script, objeto);

                throw new Exception("Ocorreu um erro de exeução de script PowerShell", new Exception(sbInnerErro.ToString()));
            }
        }
        internal static ICollection<T> ExecutarPSScriptColecao<T>(string script, T objeto, Runspace remoteRunspace) where T : class
        {
            try
            {
                using (PowerShell powershell = PowerShell.Create())
                {
                    powershell.Runspace = remoteRunspace;
                    powershell.AddScript(script);

                    ICollection<PSObject> psResults = powershell.Invoke();

                    ICollection<T> results = new List<T>();

                    if (psResults == null || psResults.Count == 0)
                    {
                        PowerShellErrosExecucao(powershell, script);
                    }

                    foreach (PSObject po in psResults)
                    {
                        if (po != null)
                        {
                            objeto = Helper.CriarInstancia(objeto.GetType()) as T;
                            SCCMHelper.PSObjectObjeto(po, objeto);
                            results.Add(objeto);
                        }
                    }

                    return results;
                }
            }
            catch (InvalidPowerShellStateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                string innerErro = PowerShellExceptionFormat(ex, script, objeto);

                throw new Exception("Ocorreu um erro de exeução de script PowerShell", new Exception(innerErro));
            }
        }
        internal static T ExecutarPSScript<T>(string script, string comando, List<Chave> param, List<ValorChave> paramValor, bool arquivo, T objeto, Runspace remoteRunspace) where T : class
        {
            try
            {
                using (PowerShell powershell = PowerShell.Create())
                {
                    string scriptTexto = script;
                    Dictionary<object, object> dctParam = new Dictionary<object, object>();

                    if (arquivo)
                        scriptTexto = System.IO.File.ReadAllText(script);

                    powershell.Runspace = remoteRunspace;
                    powershell.AddScript(scriptTexto);
                    powershell.Invoke();

                    foreach (Chave chave in param)
                    {
                        ValorChave objValor = paramValor.Where(valor => valor.IdChave == chave.IdChave)
                                                        .SingleOrDefault();

                        dctParam.Add(chave.Nome, objValor.Valor);
                    }

                    powershell.Commands.Clear();
                    powershell.AddCommand(comando).AddParameters(dctParam);

                    PSObject psObject = powershell.Invoke()
                                                  .FirstOrDefault();

                    if (psObject == null)
                    {
                        PowerShellErrosExecucao(powershell, script);
                    }

                    SCCMHelper.PSObjectObjeto(psObject, objeto);

                    return objeto;
                }
            }
            catch (InvalidPowerShellStateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                string innerErro = PowerShellExceptionFormat(ex, script, objeto);

                throw new Exception("Ocorreu um erro de exeução de script PowerShell", new Exception(innerErro));
            }
        }
        internal static ICollection<T> ExecutarPSScriptColecao<T>(string script, string comando, List<Chave> param, List<ValorChave> paramValor, bool arquivo, T objeto, Runspace remoteRunspace) where T : class
        {
            try
            {
                using (PowerShell powershell = PowerShell.Create())
                {
                    string scriptTexto = script;
                    Dictionary<object, object> dctParam = new Dictionary<object, object>();

                    if (arquivo)
                        scriptTexto = System.IO.File.ReadAllText(script);

                    powershell.Runspace = remoteRunspace;
                    powershell.AddScript(scriptTexto);
                    powershell.Invoke();

                    foreach (Chave chave in param)
                    {
                        ValorChave objValor = paramValor.Where(valor => valor.IdChave == chave.IdChave)
                                                        .SingleOrDefault();

                        dctParam.Add(chave.Nome, objValor.Valor);
                    }

                    powershell.Commands.Clear();
                    powershell.AddCommand(comando).AddParameters(dctParam);

                    ICollection<PSObject> psResults = powershell.Invoke();

                    if (psResults == null || psResults.Count == 0)
                    {
                        PowerShellErrosExecucao(powershell, script);
                    }

                    ICollection<T> results = new List<T>();

                    foreach (PSObject po in psResults)
                    {
                        if (po != null)
                        {
                            objeto = Helper.CriarInstancia(objeto.GetType()) as T;
                            SCCMHelper.PSObjectObjeto(po, objeto);
                            results.Add(objeto);
                        }
                    }

                    return results;
                }
            }
            catch (InvalidPowerShellStateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                string innerErro = PowerShellExceptionFormat(ex, script, objeto);

                throw new Exception("Ocorreu um erro de exeução de script PowerShell", new Exception(innerErro));
            }
        }
        /// <summary>
        /// Executa um script PowerShell
        /// </summary>
        /// <param name="scriptText"></param>
        /// <param name="remoteRunspace"></param>
        /// <returns></returns>
        internal static ICollection<PSObject> ExecutarPSScript(string script, string[] param, Runspace remoteRunspace)
        {
            try
            {
                using (PowerShell powershell = PowerShell.Create())
                {
                    powershell.Runspace = remoteRunspace;
                    powershell.AddScript(script);
                    powershell.AddParameters(param);

                    ICollection<PSObject> psResults = powershell.Invoke();
                    ICollection<PSObject> resultCollection = psResults.Where(t => t != null).ToList();

                    if (resultCollection == null || resultCollection.Count == 0)
                    {
                        PowerShellErrosExecucao(powershell, script);
                    }

                    return resultCollection;
                }
            }
            catch (InvalidPowerShellStateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                string innerErro = PowerShellExceptionFormat(ex, script);

                throw new Exception("Ocorreu um erro de exeução de script PowerShell", new Exception(innerErro.ToString()));
            }
        }
        /// <summary>
        /// Executa um script PowerShell
        /// </summary>
        /// <param name="scriptText"></param>
        /// <param name="remoteRunspace"></param>
        /// <returns></returns>
        internal static ICollection<PSObject> ExecutarPSScript(string script, Runspace remoteRunspace)
        {
            try
            {
                using (PowerShell powershell = PowerShell.Create())
                {
                    powershell.Runspace = remoteRunspace;
                    powershell.AddScript(script);

                    ICollection<PSObject> psResults = powershell.Invoke().Where(t => t != null)
                                                                         .ToList();

                    if (psResults == null || psResults.Count == 0)
                    {
                        PowerShellErrosExecucao(powershell, script);
                    }

                    return psResults;
                }
            }
            catch (InvalidPowerShellStateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                string innerErro = PowerShellExceptionFormat(ex, script);
                throw new Exception("Ocorreu um erro de exeução de script PowerShell", new Exception(innerErro));
            }
        }
        public static bool ValidarFuncaoExiste(string nomeFuncao, Runspace remoteRunspace)
        {
            try
            {
                PSFuncaoExisteAudit psDeletadoAudit = new PSFuncaoExisteAudit();

                string query = PS_CMD_EXITSTE_TEMPLATE.Replace("[cmd-nome]", nomeFuncao);

                ExecutarPSScript(query, psDeletadoAudit, remoteRunspace);

                return psDeletadoAudit.Existe;
            }
            catch
            {
                throw new Exception("Ocorreu um erro de requisição PowerShell", new Exception(string.Format("Erro na verificação da funcão - {0} - PowerShell.", nomeFuncao)));
            }
        }
        public static PSFuncaoExisteAudit ValidarFuncaoExisteResult(string nomeFuncao, Runspace remoteRunspace)
        {
            try
            {
                PSFuncaoExisteAudit psDeletadoAudit = new PSFuncaoExisteAudit();

                string query = PS_CMD_EXITSTE_TEMPLATE.Replace("[cmd-nome]", nomeFuncao);

                ExecutarPSScript(query, psDeletadoAudit, remoteRunspace);

                return psDeletadoAudit;
            }
            catch
            {
                throw new Exception("Ocorreu um erro de requisição PowerShell", new Exception(string.Format("Erro na verificação da funcão - {0} - PowerShell.", nomeFuncao)));
            }
        }
        public static PSObjetoRemovidoAudit DeletarQueryResult(string queryCompleto, Runspace remoteRunspace)
        {
            try
            {
                PSObjetoRemovidoAudit psDeletadoAudit = new PSObjetoRemovidoAudit();

                string wmiObject = string.Concat("$WmiObject = ", queryCompleto);

                string scriptDelete = string.Concat(wmiObject, "\n", PS_DELETE_QUERY_RESULT);

                ExecutarPSScript(scriptDelete, psDeletadoAudit, remoteRunspace);

                return psDeletadoAudit;
            }
            catch (Exception ex)
            {
                string innerErro = PowerShellExceptionFormat(ex, queryCompleto);
                throw new Exception("Ocorreu um erro de requisição de deleção PowerShell", new Exception(innerErro));
            }
        }
        /// <summary>
        /// Executa um script PowerShell
        /// </summary>
        /// <param name="scriptText"></param>
        /// <param name="remoteRunspace"></param>
        /// <returns></returns>
        internal static bool ExecutarPSScriptPUT<T>(string script, T objeto, Runspace remoteRunspace) where T : class
        {
            try
            {
                using (PowerShell powershell = PowerShell.Create())
                {
                    string scriptPUT = string.Concat(script, "\n", string.Format(PS_PUT_TEMPLATE, objeto.GetType().Name));
                    script = scriptPUT;

                    powershell.Runspace = remoteRunspace;
                    powershell.AddScript(scriptPUT);

                    PSObject psResult = powershell.Invoke()
                                                  .FirstOrDefault();


                    if (psResult == null)
                    {
                        PowerShellErrosExecucao(powershell, script);
                    }


                    return (psResult != null);
                }
            }
            catch (InvalidPowerShellStateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                string innerErro = PowerShellExceptionFormat(ex, script, objeto);
                throw new Exception("Ocorreu um erro de requisição PowerShell", new Exception(innerErro));
            }
        }
        /// <summary>
        /// Executa um script PowerShell
        /// </summary>
        /// <param name="scriptText"></param>
        /// <param name="remoteRunspace"></param>
        /// <returns></returns>
        internal static TResult ExecutarPSScriptPUTResult<T, TResult>(string script, T objeto, TResult objetoResult, Runspace remoteRunspace) where T : class
                                                                                                                                           where TResult : class
        {
            try
            {
                using (PowerShell powershell = PowerShell.Create())
                {
                    string scriptPUT = string.Concat(script, "\n", string.Format(PS_PUT_TEMPLATE, objeto.GetType().Name));

                    powershell.Runspace = remoteRunspace;
                    powershell.AddScript(scriptPUT);

                    PSObject PSResult = powershell.Invoke()
                                                  .FirstOrDefault();

                    if (PSResult == null)
                    {
                        PowerShellErrosExecucao(powershell, script);
                    }

                    SCCMHelper.PSObjectObjeto(PSResult, objetoResult);

                    return objetoResult;
                }
            }
            catch (InvalidPowerShellStateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                string innerErro = PowerShellExceptionFormat(ex, script, objeto);
                throw new Exception("Ocorreu um erro de requisição PowerShell", new Exception(innerErro));
            }
        }

        #region Metodos Privados
        private static void PowerShellErrosExecucao(PowerShell powershell, string script)
        {
            if (powershell == null)
                return;

            ICollection<ErrorRecord> psErros = powershell.Streams.Error.ReadAll();
            if (psErros != null && psErros.Count > 0)
            {
                StringBuilder sbPSErro = new StringBuilder();

                psErros.ToList().ForEach(erro =>
                {
                    sbPSErro.Append(erro.ErrorDetails != null ? erro.ErrorDetails.Message : null);
                    sbPSErro.AppendLine(erro.Exception.Message);
                    sbPSErro.Append(erro.Exception.InnerException().Message);
                    sbPSErro.Append(Environment.NewLine + "----");
                    sbPSErro.Append(string.Concat("PSScript -", script, "----Fim_PSScript"));
                });

                throw new InvalidPowerShellStateException("Execução do script powershell gerou erros", new Exception(sbPSErro.ToString().Trim()));
            }
        }
        private static string PowerShellExceptionFormat(Exception ex, string script = null, object objeto = null)
        {
            if (ex == null)
                return string.Empty;

            StringBuilder sbInnerErro = new StringBuilder();

            sbInnerErro.AppendLine(ex.InnerException().Message);

            if (objeto != null)
                sbInnerErro.AppendLine(string.Concat("Objeto - ", objeto.GetType().FullName));

            if (!string.IsNullOrEmpty(script))
                sbInnerErro.AppendLine(string.Concat("Script - ", script));

            return sbInnerErro.ToString();
        }
        #endregion
    }
}
