using HelperComum;
using SCCM.Dominio.Comum;
using SCCM.Dominio.Comum.Concreto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.WMI
{
    public partial class SMSProviderLocation
    {
        private const string PREF_CACHE = "cache-";
        private const int TEMPO_CACHE = 10080;//1 Semana

        public SMSProviderLocation SiteCodeCache()
        {
            string chave = string.Concat(PREF_CACHE, "provider");

            ICache cache = new CacheArquivoJSON(TEMPO_CACHE);

            SMSProviderLocation smsProviderCache = cache.Obter<SMSProviderLocation>(chave);

            if (smsProviderCache == null)
            {
                SMSProviderLocation[] smsProviderArray = Listar();

                if (smsProviderArray == null || smsProviderArray.Count() == 0)
                {
                    throw new InvalidOperationException("Operação inválida", new Exception("Não foi possível definir o site do SCCM"));
                }

                SMSProviderLocation smsProvider = smsProviderArray.FirstOrDefault();

                string json = smsProvider.ToJson();

                cache.Definir(json, chave);

                return smsProvider;
            }

            return smsProviderCache;
        }

        public IWMIResult SiteCodeCacheResult()
        {
            SMSProviderLocation smsProvider = SiteCodeCache();

            return Resultado<SMSProviderLocation>(smsProvider);
        }
    }
}
