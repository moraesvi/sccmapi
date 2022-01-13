using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel
{
    public class WMICSModelTemplate
    {
        public static string CSModelTemplate(string nomeClasse, List<WMIPropriedade> lstProp, bool newtonsoftJsonSerialization = false)
        {
            StringBuilder sbProp = new StringBuilder();

            string model = ObterModelTemplate(newtonsoftJsonSerialization);

            lstProp.ForEach(obj =>
            {
                sbProp.AppendLine(obj.Nome);
            });

            model = model.Replace("$NomeClasse", nomeClasse);
            model = model.Replace("$Propriedade", sbProp.ToString());

            return model;
        }

        public static string GerarArquivoCS(string nomeClasse, string sCsModel, string nomePasta)
        {
            string caminhoLocal = System.AppDomain.CurrentDomain.BaseDirectory;

            int indiceProj = caminhoLocal.IndexOf("MyBranchAPI");

            caminhoLocal = caminhoLocal.Substring(0, indiceProj);
            caminhoLocal = Path.Combine(caminhoLocal, "MyBranchAPI");

            string diretorio = Path.Combine(caminhoLocal, nomePasta);
            string arquivo = Path.Combine(diretorio, string.Concat(nomeClasse, ".cs"));

            try
            {
                if (!Directory.Exists(diretorio))
                {
                    Directory.CreateDirectory(diretorio);
                }
                if (File.Exists(arquivo))
                {
                    File.Delete(arquivo);
                }

                using (FileStream fs = File.Create(arquivo)) ;

                File.WriteAllText(arquivo, sCsModel);

                return "Model gerado com sucesso";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        private static string ObterModelTemplate(bool newtonsoftJsonSerialization)
        {
            string model = string.Empty;


            model = @"using System;
                      using System.Collections.Generic;
                      using System.Diagnostics;
                      
                      namespace SCCM.GeradorModel
                      {
                          $Json
                          public class $NomeClasse
                          {
                              $Propriedade
                          }
                      }";

            if (newtonsoftJsonSerialization)
            {
                model = model.Replace("$Json", "[JsonObject(MemberSerialization.OptIn)]");
            }
            else
            {
                model = model.Replace("$Json", "");
            }

            return model;
        }
    }
}
