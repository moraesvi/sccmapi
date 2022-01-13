using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SCCM.Dominio.Comum;
using HelperComum;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SCCM.Dominio.WMI
{
    public partial class ClientScriptApp
    {
        public IWMIResult DefinirScriptResult()
        {
            DefinirScript();

            IWMIResult result = WMIResultFactory.Criar(string.Concat("Script executado - ", this.GetType().Name), null, this);

            return result;
        }
        public ClientScriptApp DefinirScript()
        {
            try
            {
                string script = PSComando.Script;

                Chave chave = PSComando.Param.FirstOrDefault();
                ValorChave valor = ParamValor.FirstOrDefault();

                string appId = string.Concat("[string]$", chave.Nome.ToString());
                string appIdValor = valor.Valor.ToString();

                string nomeArq = Path.GetFileNameWithoutExtension(script);
                string arquivo = File.ReadAllText(script);

                arquivo = arquivo.Replace(appId, (string.Concat(appId, " = '", appIdValor, "'")));

                Script = System.Text.Encoding.UTF8.GetBytes(arquivo);

                return this;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Concat("Ocorreu um erro ", this.GetType().Name), new Exception(string.Concat("Erro na busca do script de instalação de app no client.\n\n", ex.Message)));
            }
        }
    }
}
