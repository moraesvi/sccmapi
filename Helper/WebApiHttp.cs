using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HelperComum
{
    public class WebApiHttp
    {
        public static string Requisicao(string url)
        {
            try
            {
                string html = string.Empty;

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(url);
                    HttpResponseMessage response = httpClient.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        HttpContent responseContent = response.Content;
                        html = responseContent.ReadAsStringAsync().Result;
                    }
                }

                return html;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public static string RequisicaoPost(string url, object param)
        {
            try
            {
                string html = string.Empty;

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(url);

                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    StringContent content = new StringContent(param.ToJson(), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = httpClient.PostAsync(url, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        HttpContent responseContent = response.Content;
                        html = responseContent.ReadAsStringAsync().Result;
                    }
                }

                return html;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Concat("Erro na requisição URL - ", url), ex.InnerException);
            }
        }

        public static string RequisicaoByte(string url, byte[] data)
        {
            try
            {
                string html = string.Empty;

                using (HttpClient httpClient = new HttpClient())
                {
                    object model = new { PolicyDocument = data };

                    var bSonData = SerializarBson<object>(model);

                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    ByteArrayContent byteContent = new ByteArrayContent(bSonData);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/bson");

                    httpClient.BaseAddress = new Uri(url);

                    HttpResponseMessage response = httpClient.PostAsync(url, byteContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        HttpContent responseContent = response.Content;
                        html = responseContent.ReadAsStringAsync().Result;
                    }
                }

                return html;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public static byte[] SerializarBson<T>(T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BsonDataWriter writer = new BsonDataWriter(ms))
                {
                    JsonSerializer serializer = new JsonSerializer();

                    serializer.Serialize(writer, obj);
                }

                return ms.ToArray();
            }
        }
        public static string DeserializarBoson(string base64data)
        {
            byte[] data = Convert.FromBase64String(base64data);

            using (MemoryStream ms = new MemoryStream(data))
            using (BsonDataReader reader = new BsonDataReader(ms))
            {
                JsonSerializer serializer = new JsonSerializer();

                return serializer.Deserialize<string>(reader);
            }
        }
    }
}
