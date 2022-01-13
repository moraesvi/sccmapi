using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.App.Helper
{
    public class ParseObjeto
    {
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
                            Enum enumParse = HelperComum.Helper.ParseEnum(type, PSValue);
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

                        object model = HelperComum.Helper.CriarInstancia(propType);

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

                object model = HelperComum.Helper.CriarInstancia(propType);

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
