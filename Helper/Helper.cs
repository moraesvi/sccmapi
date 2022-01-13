using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace HelperComum
{
    public class Helper
    {
        public static string ObterValorSettings(string chave, bool exception = false)
        {
            string valorSettings = ConfigurationManager.AppSettings.Get(chave);

            if (string.IsNullOrWhiteSpace(valorSettings))
            {
                if (exception)
                {
                    throw new Exception("Operação inválida", new Exception(string.Format("Não foi possível buscar ou definir o valor da chave '{0}'", chave)));
                }
            }

            return valorSettings;
        }
        public static string DominioUsuario(string dominio, string usuario)
        {
            return string.Concat(dominio, "\\", usuario);
        }
        public static bool ArrayToCSV(string[] array, string nomeArquivo)
        {       
            try
            {
                if (array == null)
                {
                    return false;
                }

                StringBuilder sbCSV = new StringBuilder();

                array.ToList().ForEach(valor =>
                {
                    sbCSV.AppendLine(valor);
                });

                string caminhoLocal = System.AppDomain.CurrentDomain.BaseDirectory;

                int indiceProj = caminhoLocal.IndexOf("MyBranchAPI");

                caminhoLocal = caminhoLocal.Substring(0, indiceProj);
                caminhoLocal = Path.Combine(caminhoLocal, "MyBranchAPI");

                nomeArquivo = Path.GetFileNameWithoutExtension(nomeArquivo);

                string diretorio = Path.Combine(caminhoLocal, "ArquivosCSV");
                string arquivo = Path.Combine(diretorio, string.Concat(nomeArquivo, ".csv"));

                if (!Directory.Exists(diretorio))
                {
                    Directory.CreateDirectory(diretorio);
                }
                if (File.Exists(arquivo))
                {
                    File.Delete(arquivo);
                }

                using (FileStream fs = File.Create(arquivo)) ;

                File.WriteAllText(arquivo, sbCSV.ToString());

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro na geração do CSV", ex.InnerException);
            }
        }       
        public static object CriarInstancia(Type type)
        {
            var ctor = type.GetConstructors()
                           .FirstOrDefault(c => c.GetParameters().Length > 0);

            if (ctor == null)
            {
                return Activator.CreateInstance(type);
            }

            var result =
                ctor.Invoke
                    (ctor.GetParameters()
                        .Select(p =>
                            p.HasDefaultValue ? p.DefaultValue :
                            p.ParameterType.IsValueType && Nullable.GetUnderlyingType(p.ParameterType) == null
                                ? Activator.CreateInstance(p.ParameterType)
                                : null
                        ).ToArray()
                    );

            return result;
        }
        public static Enum ParseEnum(Type enumType, object valor)
        {
            if (!string.IsNullOrWhiteSpace(valor.ToString()))
            {
                int intEnum = 0;
                Int32.TryParse(valor.ToString(), out intEnum);
                if (intEnum > 0)
                {
                    Enum value = Enum.ToObject(enumType, intEnum) as Enum;

                    return value;
                }
            }

            return null;
        }
        public static bool Numerico(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
        public static bool ExistePropridade<T>(T TModel, string propBuscado) where T : class
        {
            try
            {
                PropertyInfo[] propriedades = TModel.GetType().GetProperties();

                bool existe = propriedades.ToList()
                                          .Exists(objProp => string.CompareOrdinal(objProp.Name.ToUpper(), propBuscado.ToUpper()) == 0);

                return existe;
            }
            catch
            {
                throw new Exception("Ocorreu um erro na verificação da propriedade.");
            }
        }
        public static bool ExistePropridade<T>(T TModel, string[] propBuscado) where T : class
        {
            try
            {
                PropertyInfo[] propriedades = TModel.GetType().GetProperties();

                bool existe = propriedades.ToList()
                                          .Exists(objProp => propBuscado.Contains(objProp.Name, new CompararOrdinal()));

                return existe;
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception(ex.Message);
            }
            catch
            {
                throw new Exception("Ocorreu um erro na busca da propriedade.");
            }
        }
        public static string ObterPropridadeValor<T>(T TModel, string propBuscado) where T : class
        {
            try
            {
                PropertyInfo[] propriedades = TModel.GetType().GetProperties();

                PropertyInfo prop = propriedades.ToList()
                                                .Where(objProp => string.CompareOrdinal(objProp.Name.ToUpper(), propBuscado.ToUpper()) == 0)
                                                .FirstOrDefault();

                if (prop == null)
                    throw new InvalidOperationException("Propriedade não encontrada.");

                return prop.GetValue(TModel).ToString();
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception(ex.Message);
            }
            catch
            {
                throw new Exception("Ocorreu um erro na busca da propriedade.");
            }
        }
        public static T ObterPropridade<T>(PropertyInfo[] propriedades, string propBuscado) where T : class
        {
            try
            {
                PropertyInfo prop = propriedades.ToList()
                                                .Where(objProp => string.CompareOrdinal(objProp.Name.ToUpper(), propBuscado.ToUpper()) == 0)
                                                .FirstOrDefault();

                if (prop == null)
                    throw new InvalidOperationException("Propriedade não encontrada.");

                T propriedade = prop.GetValue(propriedades) as T;

                return propriedade;
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception(ex.Message);
            }
            catch
            {
                throw new Exception("Ocorreu um erro na busca da propriedade.");
            }
        }
    }
    public class CompararOrdinal : IEqualityComparer<string>
    {
        public bool Equals(string valor1, string valor2)
        {
            if (string.CompareOrdinal(valor1.ToUpper(), valor2.ToUpper()) == 0)
            {
                return true;
            }

            return false;
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }

    public class CompararIndexOf : IEqualityComparer<string>
    {
        public bool Equals(string valor1, string valor2)
        {
            if (valor1.IndexOf(valor2, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            return false;
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
}
