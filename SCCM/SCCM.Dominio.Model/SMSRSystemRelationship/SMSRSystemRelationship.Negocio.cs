using HelperComum;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SCCM.Dominio.Comum;
using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Model
{
    public partial class SMSRSystemRelationship
    {
        public IWMIResult ListarCacheResult()
        {
            SMSRSystemRelationship[] cacheSalvo = _cache.ObterArray<SMSRSystemRelationship>(string.Concat(PREF_CACHE, "dispositivos"));

            if (cacheSalvo == null)
            {
                cacheSalvo = Cache();
            }

            return Resultado(cacheSalvo);
        }
        public SMSRSystemRelationship[] ListarCache()
        {
            SMSRSystemRelationship[] cacheSalvo = _cache.ObterArray<SMSRSystemRelationship>(string.Concat(PREF_CACHE, "dispositivos"));

            if (cacheSalvo == null)
            {
                cacheSalvo = Cache();
            }

            return cacheSalvo;
        }
        public IWMIResult ObterDispositivoResult(string dominio, string usuario)
        {
            string chaveCache = string.Concat(dominio, "_", usuario);
            string dominioUsuario = SCCMHelper.FormatDominioUsuarioPSParam(dominio, usuario);

            string where = WHERE_USUARIO.Replace("$usuario", dominioUsuario);

            PSQuery.AddWhere(where);

            string script = PSQuery.ToString();

            SMSRSystemRelationship[] cacheSalvo = _cache.ObterArray<SMSRSystemRelationship>(string.Concat(PREF_CACHE, "dispositivos-filtro-", chaveCache));

            if (cacheSalvo == null)
            {
                cacheSalvo = Cache(script, chaveCache);
            }

            return Resultado(cacheSalvo);
        }
        public SMSRSystemRelationship[] ObterDispositivo(string dominio, string usuario)
        {
            string chaveCache = string.Concat(dominio, "_", usuario);
            string dominioUsuario = SCCMHelper.FormatDominioUsuarioPSParam(dominio, usuario);

            string where = WHERE_USUARIO.Replace("$usuario", dominioUsuario);

            PSQuery.AddWhere(where);

            string script = PSQuery.ToString();

            SMSRSystemRelationship[] smsRSystem = PSRequisicao.ExecutarColecao(script, this);

            return smsRSystem;
        }
        public SMSRSystemRelationship[] ObterDispositivosColecao(string idColecao, bool gerarCache = false)
        {
            string where = WHERE_COLECAO.Replace("$idColecao", idColecao);

            PSQuery.AddWhere(where);

            string script = PSQuery.ToString();

            SMSRSystemRelationship[] dispositivos = PSRequisicao.ExecutarColecao(script, this);

            return dispositivos;
        }
        public bool GerarCache()
        {
            SMSRSystemRelationship[] smsRSystem = this.Listar();

            string json = smsRSystem.ToJson();

            bool definido = _cache.Definir(json, string.Concat(PREF_CACHE, "dispositivos"));

            return definido;
        }
        public bool GerarCache(string dominio, string usuario)
        {
            string chaveCache = string.Concat(dominio, "_", usuario);
            string dominioUsuario = SCCMHelper.FormatDominioUsuarioPSParam(dominio, usuario);

            string script = PSQuery.ToString().Replace("$usuario", dominioUsuario);

            SMSRSystemRelationship[] smsRSystem = PSRequisicao.ExecutarColecao(script, this);

            string json = smsRSystem.ToJson();

            bool definido = _cache.Definir(json, string.Concat(PREF_CACHE, "dispositivos-filtro-", chaveCache));

            return definido;
        }
        private SMSRSystemRelationship[] Cache()
        {

            SMSRSystemRelationship[] smsRSystem = this.Listar();

            string json = smsRSystem.ToJson();

            _cache.Definir(json, string.Concat(PREF_CACHE, "dispositivos"));

            smsRSystem = _cache.ObterArray<SMSRSystemRelationship>(string.Concat(PREF_CACHE, "dispositivos"));

            return smsRSystem;
        }
        private SMSRSystemRelationship[] Cache(string script, string chave)
        {
            SMSRSystemRelationship[] smsRSystem = PSRequisicao.ExecutarColecao(script, this);

            string json = smsRSystem.ToJson();

            _cache.Definir(json, string.Concat(PREF_CACHE, "dispositivos-filtro-", chave));

            smsRSystem = _cache.ObterArray<SMSRSystemRelationship>(string.Concat(PREF_CACHE, "dispositivos-filtro-", chave));

            return smsRSystem;
        }
    }
}
