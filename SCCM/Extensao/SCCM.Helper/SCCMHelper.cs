using HelperComum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SCCM
{
    public class SCCMHelper
    {
        public static string SMSSiteNamespace
        {
            get
            {
                string site = System.Configuration.ConfigurationManager.AppSettings.Get("SCCM_SITE");
                site = string.IsNullOrWhiteSpace(site) ? "PR1" : site;

                return string.Concat("root\\SMS\\Site_", site.ToUpper());
            }
        }
        public static string SMSNamespace
        {
            get { return "root\\SMS"; }
        }
        public static string CCMNamespace
        {
            get { return "root\\CCM"; }
        }
        public static string SCCMSDKNamespace
        {
            get { return "root\\ccm\\ClientSDK"; }
        }
        public static string CCMActualConfigNamespace
        {
            get { return "root\\ccm\\policy\\machine\\ActualConfig"; }
        }
        public static string CCMRequestedConfigNamespace
        {
            get { return "root\\ccm\\policy\\machine\\RequestedConfig"; }
        }
        public static string SMSSite
        {
            get
            {
                string site = System.Configuration.ConfigurationManager.AppSettings.Get("SCCM_SITE");
                site = string.IsNullOrWhiteSpace(site) ? "PR1" : site;

                return site;
            }
        }
        public static string FormatDominioUsuarioPSParam(string dominioUsuarioCompleto)
        {
            try
            {
                if (string.IsNullOrEmpty(dominioUsuarioCompleto))
                    return dominioUsuarioCompleto;

                string dominioUsuarioFormat = string.Empty;
                string[] dominioUsuario = dominioUsuarioCompleto.Split(new char[] { '\\' });

                if (dominioUsuario.Length == 2)
                {
                    dominioUsuarioFormat = dominioUsuarioCompleto.Replace("\\", "\\\\");

                    return dominioUsuarioFormat;
                }

                return dominioUsuarioCompleto;
            }
            catch
            {
                throw new Exception("Ocorreu um erro na formatação do parametro Dominio\\Usuario.");
            }
        }
        public static string FormatDominioUsuarioPSParam(string dominio, string usuario)
        {
            try
            {
                if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(dominio))
                    return usuario;

                string dominioUsuarioFormat = string.Empty;

                dominioUsuarioFormat = string.Concat(dominio, "\\\\", usuario);

                return dominioUsuarioFormat;
            }
            catch
            {
                throw new Exception("Ocorreu um erro na formatação do parametro Dominio\\Usuario.");
            }
        }
        private static void PSPropriedadeValor<T>(object PSValue, PropertyInfo propriedade, Type tpPPS, T TModel)
        {
            if (propriedade != null && PSValue != null)
            {
                if (tpPPS == null)
                {
                    if (propriedade.PropertyType.IsArray)
                    {
                        Array array = Array.CreateInstance(propriedade.PropertyType.GetElementType(), 1);
                        Type type = array.GetType().GetElementType();

                        if (type == typeof(string))
                            array.SetValue(PSValue.ToString(), 0);
                        else if (type.IsEnum)
                        {
                            Enum enumParse = Helper.ParseEnum(type, PSValue);
                            if (enumParse != null)
                                array.SetValue(enumParse, 0);
                        }
                        else
                            array.SetValue(Convert.ChangeType(PSValue.ToString(), type), 0);

                        propriedade.SetValue(TModel, array);
                    }
                }
                else if (tpPPS.GetType() == typeof(Enum))
                {
                    propriedade.SetValue(TModel, Enum.ToObject(propriedade.PropertyType, PSValue), null);
                }
                else if (!string.IsNullOrWhiteSpace(PSValue.ToString()))
                {
                    propriedade.SetValue(TModel, PSValue);
                }
            }
        }
        private static void PSPropriedadeValor<T>(PSObject PSObjeto, PropertyInfo propriedade, Type tpPPS, T TModel)
        {
            PSObjeto.Properties.ToList().ForEach(psProp =>
            {
                if (propriedade != null && psProp.Value != null)
                {
                    tpPPS = Type.GetType(psProp.Value.GetType().FullName, false, true);

                    if (tpPPS == null)
                    {
                        if (propriedade.PropertyType.IsArray)
                        {
                            Array array = Array.CreateInstance(propriedade.PropertyType.GetElementType(), 1);
                            Type arrayType = array.GetType().GetElementType();

                            if (arrayType == typeof(string))
                                array.SetValue(psProp.Value.ToString(), 0);
                            else
                                array.SetValue(Convert.ChangeType(psProp.Value.ToString(), arrayType), 0);

                            propriedade.SetValue(TModel, array);
                        }
                    }
                    else if (tpPPS.GetType() == typeof(Enum))
                    {
                        propriedade.SetValue(TModel, Enum.ToObject(propriedade.PropertyType, psProp.Value), null);
                    }
                    else if (!string.IsNullOrWhiteSpace(psProp.Value.ToString()))
                    {
                        propriedade.SetValue(TModel, psProp.Value);
                    }
                }
            });
        }
        private static void PSPropriedadeValor<T>(ArrayList colecaoPsObjeto, PropertyInfo propriedade, T TModel)
        {
            try
            {
                Type propType = propriedade.PropertyType.GetElementType();
                Array arrayResult = null;

                if (propType.IsClass)
                {
                    Type arrayType = propriedade.PropertyType.GetElementType();
                    arrayResult = Array.CreateInstance(arrayType, colecaoPsObjeto.Count);

                    for (int indice = 0; indice < colecaoPsObjeto.Count; indice++)
                    {
                        object item = colecaoPsObjeto[indice];

                        Hashtable hashtableModel = (item as PSObject).BaseObject as Hashtable;

                        object model = Helper.CriarInstancia(propType);

                        if (hashtableModel != null)
                        {
                            PSObjectObjeto(hashtableModel, propriedade, model, TModel);
                            arrayResult.SetValue(model, indice);
                            continue;
                        }

                        PSObject PSobj = (item as PSObject);

                        if (PSobj != null)
                        {
                            PSObjectObjeto(PSobj, model);
                            continue;
                        }
                    }

                    propriedade.SetValue(TModel, arrayResult);
                }
            }
            catch (Exception ex)
            {

            }
        }
        private static void PSPropriedadeValor<T>(object item, PropertyInfo propriedade, T TModel)
        {
            Type propType = propriedade.PropertyType;

            if (propType.IsClass)
            {
                Hashtable hashtableModel = item as Hashtable;

                object model = Helper.CriarInstancia(propType);

                if (hashtableModel != null)
                {
                    PSObjectObjeto(hashtableModel, propriedade, model, TModel);
                }

                PSObject PSobj = (item as PSObject);

                if (PSobj != null)
                {
                    PSObjectObjeto(PSobj, model);
                }

                propriedade.SetValue(TModel, model);
            }
        }
        public static void PSObjectObjeto<T>(PSObject PSobj, T TModel)
        {
            try
            {
                if (PSobj != null && TModel != null)
                {
                    PropertyInfo[] propriedades = TModel.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                    PSobj.Properties.ToList().ForEach(psProp =>
                    {
                        PropertyInfo objPropriedade = propriedades.ToList().Where(objProp => string.CompareOrdinal(objProp.Name.ToUpper(), psProp.Name.ToUpper()) == 0)
                                                                           .FirstOrDefault();

                        ProcessarPSObjectObjeto(psProp.Value, objPropriedade, TModel);
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static object PSObjectObjeto<T>(Hashtable hashtableModel, PropertyInfo propriedade, object model, T TModel)
        {
            try
            {
                if (hashtableModel != null && model != null)
                {
                    PropertyInfo[] propriedades = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                    Type arrayType = propriedade.PropertyType.GetElementType();

                    foreach (DictionaryEntry entry in hashtableModel)
                    {
                        PropertyInfo objPropriedade = propriedades.ToList().Where(objProp => string.CompareOrdinal(objProp.Name.ToUpper(), entry.Key.ToString().ToUpper()) == 0)
                                                                           .FirstOrDefault();

                        ProcessarPSObjectObjeto(entry.Value, objPropriedade, model);
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void ProcessarPSObjectObjeto<T>(object PSValue, PropertyInfo objPropriedade, T TModel)
        {
            try
            {
                if (objPropriedade != null && PSValue != null)
                {
                    Type tpPPS = Type.GetType(PSValue.GetType().FullName, false, true);

                    if (tpPPS == null)
                    {
                        PSObject psObjetoModel = PSValue as PSObject;

                        if (!psObjetoModel.PossuiValor())
                            return;

                        if (psObjetoModel.Array() && objPropriedade.PropertyType.IsArray && objPropriedade.PropertyType.GetElementType().IsClass)
                        {
                            ArrayList arrayPsObjeto = psObjetoModel.BaseObject as ArrayList;

                            if (arrayPsObjeto != null)
                            {
                                PSPropriedadeValor(arrayPsObjeto, objPropriedade, TModel);
                            }
                            else
                            {
                                PSPropriedadeValor(psObjetoModel.BaseObject, objPropriedade, TModel);
                            }
                        }
                        else
                        {
                            if (objPropriedade.PropertyType.IsArray)
                            {
                                PSPropriedadeValor(PSValue, objPropriedade, tpPPS, TModel);
                            }
                            else if (objPropriedade.PropertyType.IsClass)
                            {
                                PSPropriedadeValor(psObjetoModel.BaseObject, objPropriedade, TModel);
                            }
                        }
                    }
                    else
                    {
                        PSPropriedadeValor(PSValue, objPropriedade, tpPPS, TModel);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string ToPowershell(object objeto, string PSVariavel, bool powerShellModel)
        {
            try
            {
                if (objeto == null)
                    return string.Empty;

                string psFechaArray = ")";

                StringBuilder sbPowerShell = new StringBuilder();
                PropertyInfo[] propriedades = objeto.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                PropertyInfo[] propPsInstance = objeto.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                                                                .Where(prop => prop.PropertyType.IsClass && prop.PropertyType.IsArray)
                                                                .ToArray();


                #region Define Poweshell arrays internos em classe

                foreach (PropertyInfo prop in propPsInstance)
                {
                    string scriptModelPS = string.Empty;
                    string nomeClasse = string.Empty;
                    StringBuilder sbPS = new StringBuilder();

                    if (prop.PropertyType.IsArray)
                    {
                        Array array = (prop.GetValue(objeto, null) as Array);

                        if (array == null)
                            continue;

                        string varArray = string.Empty;

                        if (array.Length > 0)
                        {
                            object objetoProp = array.GetValue(0);

                            if (!objetoProp.GetType().IsClass)//Somente array de classe
                                continue;

                            varArray = string.Concat("$", objetoProp.GetType().Name, "Array");
                            string varArrayInstancia = string.Concat(varArray, " = @()");

                            sbPowerShell.AppendLine(varArrayInstancia);
                        }

                        for (int indice = 0; indice < array.Length; indice++)
                        {
                            object objetoProp = array.GetValue(indice);
                            PropertyInfo[] propriedadesArray = objetoProp.GetType().GetProperties();

                            string PSVariavelInstance = string.Concat(objetoProp.GetType().Name);

                            MethodInfo metodo = objetoProp.GetType().GetMethods()
                                                          .Where(objMetodo => objMetodo.Name.IndexOf("PSInstance", StringComparison.OrdinalIgnoreCase) >= 0)
                                                          .FirstOrDefault();
                            if (metodo == null)
                            {
                                throw new InvalidOperationException("Ocorreu um erro na serialização PowerShell", new Exception(string.Concat("Não foi possível obter uma instância da classe: ", PSVariavelInstance)));
                            }

                            sbPowerShell.AppendLine("");

                            string instanciaClassePS = scriptModelPS = metodo.Invoke(objetoProp, null) as string;
                            sbPowerShell.AppendLine(instanciaClassePS);

                            scriptModelPS = PropriedadeValor(propriedadesArray, objetoProp, PSVariavelInstance);

                            PSVariavelInstance = string.Concat("$", PSVariavelInstance);

                            if (!string.IsNullOrEmpty(scriptModelPS))
                            {
                                if (indice == 0)
                                {
                                    sbPowerShell.AppendLine(scriptModelPS);
                                    sbPowerShell.AppendLine(string.Concat(varArray, " += ", PSVariavelInstance));
                                    continue;
                                }

                                sbPowerShell.AppendLine(string.Concat("\n,", scriptModelPS));
                            }

                            sbPowerShell.AppendLine(string.Concat(varArray, " += ", PSVariavelInstance));
                        }
                    }
                    else if (prop.PropertyType.IsClass)
                    {
                        object objetoProp = prop.GetValue(objeto, null);
                        PropertyInfo[] propriedadesArray = objetoProp.GetType().GetProperties();

                        string PSVariavelInstance = objetoProp.GetType().Name;

                        MethodInfo metodo = objetoProp.GetType().GetMethods()
                                                      .Where(objMetodo => objMetodo.Name.IndexOf("PSInstance", StringComparison.OrdinalIgnoreCase) >= 0)
                                                      .FirstOrDefault();
                        if (metodo == null)
                        {
                            throw new InvalidOperationException("Ocorreu um erro na serialização PowerShell", new Exception(string.Concat("Não foi possível obter uma instância da classe: ", PSVariavelInstance)));
                        }

                        sbPowerShell.AppendLine("");

                        scriptModelPS = metodo.Invoke(objetoProp, null) as string;
                        sbPowerShell.AppendLine(scriptModelPS);

                        PropriedadeValor(propriedadesArray, objetoProp, PSVariavelInstance, sbPowerShell);
                    }
                }

                #endregion

                #region Define Powershell de array de classe

                if (objeto.GetType().IsArray && !powerShellModel)
                {
                    string scriptModelPS = string.Empty;

                    Array array = (objeto as Array);

                    if (array.Length > 0)
                    {
                        object objetoInstancia = array.GetValue(0);

                        MethodInfo metodo = objetoInstancia.GetType().GetMethods()
                                                           .Where(objMetodo => objMetodo.Name.IndexOf("PSInstance", StringComparison.OrdinalIgnoreCase) >= 0)
                                                           .FirstOrDefault();
                        if (metodo != null)
                        {
                            //Instância da classe
                            scriptModelPS = metodo.Invoke(objetoInstancia, null) as string;
                            sbPowerShell.AppendLine(scriptModelPS);
                        }
                    }

                    for (int indice = 0; indice < array.Length; indice++)
                    {
                        object objetoProp = array.GetValue(indice);
                        propriedades = objetoProp.GetType().GetProperties();

                        StringBuilder sbProp = new StringBuilder();
                        scriptModelPS = PropriedadeValor(propriedades, objetoProp, PSVariavel, sbProp);

                        if (!string.IsNullOrEmpty(scriptModelPS))
                        {
                            if (indice == 0)
                            {
                                sbPowerShell.Append(scriptModelPS.ToString());
                                continue;
                            }
                            sbPowerShell.Append(string.Concat(",\n", scriptModelPS));
                        }
                    }

                    sbPowerShell.AppendLine(psFechaArray);
                }
                else if (objeto.GetType().IsArray && powerShellModel)
                {
                    string scriptModelPS = string.Empty;
                    string nomePropriedade = string.Empty;
                    Array array = (objeto as Array);

                    string varArray = string.Concat("$", objeto.GetType().GetElementType().Name, "Array");
                    string varArrayInstancia = string.Concat(varArray, " = @()");

                    sbPowerShell.AppendLine(varArrayInstancia);

                    for (int indice = 0; indice < array.Length; indice++)
                    {
                        object objetoProp = array.GetValue(indice);
                        propriedades = objetoProp.GetType().GetProperties();

                        nomePropriedade = string.Concat("prop_", indice);

                        StringBuilder sbProp = new StringBuilder();
                        scriptModelPS = PropriedadeValorPSModel(propriedades, objetoProp, PSVariavel, nomePropriedade, sbProp);

                        if (!string.IsNullOrEmpty(scriptModelPS))
                        {
                            sbPowerShell.AppendLine(string.Concat(varArray, " += ", scriptModelPS));
                        }
                    }
                }

                #endregion           

                else
                {
                    PropriedadeValor(propriedades, objeto, PSVariavel, sbPowerShell);
                }

                return sbPowerShell.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static string PropriedadeValor(PropertyInfo[] propriedades, object objeto, string PSVariavel, StringBuilder sbPowerShell = null)
        {
            string psAbreArray = "@(";
            string psFechaArray = ")";

            if (sbPowerShell == null)
            {
                sbPowerShell = new StringBuilder();
            }

            sbPowerShell.AppendLine("");

            foreach (PropertyInfo prop in propriedades)
            {
                string psProp = string.Empty;

                object valor = prop.GetValue(objeto, null);

                if (valor == null)
                    continue;

                if (Helper.Numerico(valor.GetType()))
                {
                    if (string.CompareOrdinal(valor.ToString(), "0") == 0)
                        continue;
                }

                if (!prop.PropertyType.IsArray)
                {
                    psProp = ValorPropriedadePSModel(valor, PSVariavel, prop);
                    psProp = string.Concat("$", PSVariavel, ".", psProp);
                }
                else
                {
                    Array array = (valor as Array);
                    Type type = array.GetType().GetElementType();

                    StringBuilder psSbArray = new StringBuilder();

                    psSbArray.Append(psAbreArray);

                    for (int indice = 0; indice < array.Length; indice++)
                    {
                        valor = array.GetValue(indice);

                        if (type.IsClass)//Caso não seja model, irá criar instância da classe do array
                        {
                            valor = string.Concat("$", string.Concat(valor.GetType().Name, "Array"));
                            psSbArray.Append(valor);
                            continue;
                        }

                        if (indice == 0)
                        {
                            valor = ValorPropriedadeArray(valor, PSVariavel, prop);

                            psSbArray.Append(valor);
                            continue;
                        }

                        valor = ValorPropriedadeArray(valor, PSVariavel, prop);
                        psSbArray.Append(valor);
                    }

                    valor = string.Concat(psSbArray.ToString(), psFechaArray);
                    psProp = string.Concat("$", PSVariavel, ".", prop.Name, " = ", valor);
                }

                sbPowerShell.AppendLine(psProp);
            }

            return sbPowerShell.ToString();
        }
        private static string PropriedadeValorPSModel(PropertyInfo[] propriedades, object objeto, string PSVariavel, string nomePropriedade, StringBuilder sbPowerShell)
        {
            string psAbreProp = "@{";
            string psFechaProp = "}";

            StringBuilder sbProp = new StringBuilder();

            foreach (PropertyInfo prop in propriedades)
            {
                string psProp = string.Empty;

                object valor = prop.GetValue(objeto, null);

                if (valor == null)
                    continue;

                if (Helper.Numerico(valor.GetType()))
                {
                    if (string.CompareOrdinal(valor.ToString(), "0") == 0)
                        continue;
                }

                if (!prop.PropertyType.IsArray)
                {
                    psProp = ValorPropriedadePSModel(valor, PSVariavel, prop);
                }

                sbProp.AppendLine(psProp);
            }

            if (!string.IsNullOrEmpty(sbProp.ToString()))
            {
                sbPowerShell.AppendLine(string.Concat("$", nomePropriedade, " = ", psAbreProp));
                sbPowerShell.AppendLine(sbProp.ToString());
                sbPowerShell.Append(psFechaProp);
            }

            return sbPowerShell.ToString();
        }
        private static string ValorPropriedade(object valor, string PSVariavel, PropertyInfo prop)
        {
            string psProp = string.Empty;

            if (valor.GetType() == typeof(string))
            {
                psProp = string.Concat("$", PSVariavel, ".", prop.Name, " = \"", valor.ToString(), "\"");
            }
            else if (valor.GetType() == typeof(bool))
            {
                psProp = string.Concat("$", PSVariavel, ".", prop.Name, " = $", valor.ToString().ToLower());
            }
            else if (valor.GetType() == typeof(DateTime))
            {
                DateTime data = Convert.ToDateTime(valor);
                psProp = string.Concat("$", PSVariavel, ".", prop.Name, " = \"", data.ToDateTimePowerShell(), "\"");
            }
            else if (valor.GetType().IsClass && !valor.GetType().IsArray)
            {
                psProp = string.Concat("$", PSVariavel, ".", prop.Name, " = $", valor.GetType().Name.ToString().ToLower());
            }
            else if (Helper.Numerico(valor.GetType()))
            {
                psProp = string.Concat("$", PSVariavel, ".", prop.Name, " = ", valor);
            }

            return psProp;
        }
        private static string ValorPropriedadePSModel(object valor, string PSVariavel, PropertyInfo prop)
        {
            string psProp = string.Empty;

            if (valor.GetType() == typeof(string))
            {
                psProp = string.Concat(prop.Name, " = \"", valor.ToString(), "\"");
            }
            else if (valor.GetType() == typeof(bool))
            {
                psProp = string.Concat(prop.Name, " = $", valor.ToString().ToLower());
            }
            else if (valor.GetType() == typeof(DateTime))
            {
                DateTime data = Convert.ToDateTime(valor);
                psProp = string.Concat(prop.Name, " = \"", data.ToDateTimePowerShell(), "\"");
            }
            else if (valor.GetType().IsClass && !valor.GetType().IsArray)
            {
                psProp = string.Concat(prop.Name, " = $", valor.GetType().Name.ToString().ToLower());
            }
            else if (Helper.Numerico(valor.GetType()))
            {
                psProp = string.Concat(prop.Name, " = ", valor);
            }

            return psProp;
        }
        private static string ValorPropriedadeArray(object valor, string PSVariavel, PropertyInfo prop)
        {
            string psProp = string.Empty;

            if (valor.GetType() == typeof(string))
            {
                psProp = string.Concat("\"", valor.ToString(), "\"");
            }
            else if (valor.GetType() == typeof(bool))
            {
                psProp = string.Concat("$", valor.ToString());
            }
            else if (valor.GetType() == typeof(DateTime))
            {
                DateTime data = Convert.ToDateTime(valor);
                psProp = string.Concat("\"", data.ToDateTimePowerShell(), "\"");
            }
            else if (Helper.Numerico(valor.GetType()))
            {
                psProp = string.Concat(valor);
            }

            return psProp;
        }
        private static string ValorPropriedadePSModel(object valor, PropertyInfo prop)
        {
            string psModel = string.Empty;

            if (valor.GetType() == typeof(string))
            {
                psModel = string.Concat(prop.Name, " = \"", valor.ToString(), "\"");
            }
            else if (valor.GetType() == typeof(bool))
            {
                psModel = string.Concat(prop.Name, " = $", valor.ToString().ToLower());
            }
            else if (valor.GetType() == typeof(DateTime))
            {
                DateTime data = Convert.ToDateTime(valor);
                psModel = string.Concat(prop.Name, " = \"", data.ToDateTimePowerShell(), "\"");
            }
            else if (valor.GetType().IsClass && !valor.GetType().IsArray)
            {
                psModel = string.Concat(prop.Name, " = $", valor.GetType().Name.ToString().ToLower());
            }
            else if (Helper.Numerico(valor.GetType()))
            {
                psModel = string.Concat(prop.Name, " = ", valor);
            }

            return psModel;
        }
    }

    public static class SCCMExtesions
    {
        public static string ToDateTimePowerShell(this DateTime datetime)
        {
            string sDtPowerShell = string.Concat(datetime.ToString("yyyyMMddHHmmss"), ".000000+***");

            return sDtPowerShell;
        }
        public static bool Array(this PSObject PSObject)
        {
            if (PSObject == null)
                return false;

            string psObjectDesc = PSObject.ToString().Split()[0];

            Type type = Type.GetType(psObjectDesc, false, true);

            if (type != null)
            {
                if ((PSObject.BaseObject as ArrayList) != null || (PSObject.BaseObject as Hashtable) != null)
                    return true;
            }

            return false;
        }
        public static bool Classe(this PropertyInfo propriedade)
        {
            if (propriedade == null)
                return false;

            if (propriedade.GetType().GetElementType().IsClass)
            {
                return true;
            }

            return false;
        }
        public static bool PossuiValor(this PSObject PSObject)
        {
            if (!string.IsNullOrWhiteSpace(PSObject.ToString()))
            {
                return true;
            }

            return false;
        }
    }
}
