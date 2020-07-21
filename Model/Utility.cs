using Genesyslab.Desktop.Modules.Core.Model.Interactions;
using Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Tipificaciones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Model
{
    public class Utility
    {
        public static Type GetTypeFromSimpleName(string typeName)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            bool isArray = false, isNullable = false;

            if (typeName.IndexOf("[]") != -1)
            {
                isArray = true;
                typeName = typeName.Remove(typeName.IndexOf("[]"), 2);
            }

            if (typeName.IndexOf("?") != -1)
            {
                isNullable = true;
                typeName = typeName.Remove(typeName.IndexOf("?"), 1);
            }

            typeName = typeName.ToLower();

            string parsedTypeName = null;
            switch (typeName)
            {
                case "bool":
                case "boolean":
                    parsedTypeName = "System.Boolean";
                    break;

                case "byte":
                    parsedTypeName = "System.Byte";
                    break;

                case "char":
                    parsedTypeName = "System.Char";
                    break;

                case "datetime":
                    parsedTypeName = "System.DateTime";
                    break;

                case "datetimeoffset":
                    parsedTypeName = "System.DateTimeOffset";
                    break;

                case "decimal":
                    parsedTypeName = "System.Decimal";
                    break;

                case "double":
                    parsedTypeName = "System.Double";
                    break;

                case "float":
                    parsedTypeName = "System.Single";
                    break;

                case "int16":
                case "short":
                    parsedTypeName = "System.Int16";
                    break;

                case "int32":
                case "int":
                    parsedTypeName = "System.Int32";
                    break;

                case "int64":
                case "long":
                    parsedTypeName = "System.Int64";
                    break;

                case "object":
                    parsedTypeName = "System.Object";
                    break;

                case "sbyte":
                    parsedTypeName = "System.SByte";
                    break;

                case "string":
                    parsedTypeName = "System.String";
                    break;

                case "timespan":
                    parsedTypeName = "System.TimeSpan";
                    break;

                case "uint16":
                case "ushort":
                    parsedTypeName = "System.UInt16";
                    break;

                case "uint32":
                case "uint":
                    parsedTypeName = "System.UInt32";
                    break;

                case "uint64":
                case "ulong":
                    parsedTypeName = "System.UInt64";
                    break;

                case "checkbox":
                    parsedTypeName = "System.Windows.Controls.CheckBox";
                    break;

                case "textbox":
                    parsedTypeName = "System.Windows.Controls.TextBox";
                    break;

                case "label":
                    parsedTypeName = "System.Windows.Controls.Label";
                    break;
            }

            if (parsedTypeName != null)
            {
                if (isArray)
                    parsedTypeName = parsedTypeName + "[]";

                if (isNullable)
                    parsedTypeName = string.Concat("System.Nullable`1[", parsedTypeName, "]");
            }
            else
                parsedTypeName = typeName;

            // Expected to throw an exception in case the type has not been recognized.
            return Type.GetType(parsedTypeName);
        }

        public static T FindChildControl<T>(DependencyObject parentDepObj) where T : DependencyObject
        {
            T child = null;

            for (var index = 0; index < VisualTreeHelper.GetChildrenCount(parentDepObj); index++)
            {
                var depObj = VisualTreeHelper.GetChild(parentDepObj, index);

                if (depObj is T)
                    child = depObj as T;
                else if (VisualTreeHelper.GetChildrenCount(depObj) > 0)
                    child = FindChildControl<T>(depObj);

                if (child != null)
                    break;
            }

            return child;
        }

        //Metodo que rescata parte de NUMEROS de una cadena de texto
        public static string TRAE_NUMEROS_CADENAS(string input)
        {
            var result = Regex.Replace(input, @"[^\d]", "");
            return result;
        }

        //Metodo que rescata parte de texto de una cadena de texto
        public static string TRAE_TEXTO_CADENAS(string input)
        {
            var result = Regex.Replace(input, @"[^a-zA-ZáéíóúAÉÍÓÚÑñ ]", "");
            return result;
        }

        public static XDocument GeneraXML(IInteraction inx)
        {
            try
            {
                List<string> lista = new List<string>(inx.GetAllAttachedData().AllKeys);
                string data = "";

                XDocument Documento = new XDocument(new XElement("Campana"));

                foreach (string Key in lista)
                {
                    if (!Key.Equals("GSW_TZ_OFFSET") &&
                        !Key.Equals("GSW_PHONE") &&
                        !Key.Equals("GSW_CALLING_LIST") &&
                        !Key.Equals("GSW_CAMPAIGN_NAME") &&
                        !Key.Equals("InteractionType") &&
                        !Key.Equals("InteractionSubtype") &&
                        !Key.Equals("GSW_RECORD_HANDLE") &&
                        !Key.Equals("GSW_APPLICATION_ID") &&
                        !Key.Equals("GSW_CAMPAIGN_GROUP_DBID") &&
                        !Key.Equals("GSW_CALLING_LIST_DBID") &&
                        !Key.Equals("GSW_CAMPAIGN_GROUP_NAME") &&
                        !Key.Equals("GSW_CAMPAIGN_GROUP_DESCRIPTION") &&
                        !Key.Equals("GSW_TZ_NAME") &&
                        !Key.Equals("GSW_RECORD_URI") &&
                        !Key.Equals("GSW_CALL_ATTEMPT_GUID") &&
                        !Key.Equals("GSW_CONTACT_MEDIA_TYPE") &&
                        !Key.Equals("GSW_SWITCH_DBID") &&
                        !Key.Equals("1-TIPO_DISCADO"))
                    {
                        data = inx.GetAttachedData(Key) == null ? "" : inx.GetAttachedData(Key).ToString();
                        XElement add = new XElement(Key, data);
                        Documento.Element("Campana").Add(add);
                    }
                }

                return Documento;
            }
            catch (Exception ex)
            {
                return null;
            }


        }



        public static XDocument GeneraDatosDemograficos(Grid gridDemograficas,Expander exp)
        {
            XElement Documento = new XElement("XML");

            XElement child = new XElement(exp.Name);
            bool texto = false;
            try
            {
                foreach (var item in gridDemograficas.Children)
                {
                    TextBox textbox = item as TextBox;
                    if (textbox != null && textbox.Name != null )
                    {
                        if(textbox.Text  != "")
                          texto = true;

                       child.Add(new XElement(textbox.Name, textbox.Text));
                    }
                   
                 }
                if (texto)
                {
                    Documento.Add(child);
                }
                else
                {
                    return null;
                }
                  
            }
            
            catch (Exception ex)
            {

                return null;
            }

            XDocument doc = new XDocument(Documento);
            return doc;
        }

     

        public static XDocument GeneraTipificacion(List<TipificacionItem> listaSeleccion)
        {
            XElement Documento = new XElement("XML");

            XElement child = new XElement("TIPIFICACION");
            try
            {
                foreach (var item in listaSeleccion)
                {
                    
                   child.Add(new XElement("TIPIFICACION_ID", item.Value));
                    
                }
                if (listaSeleccion.Count > 0)
                    Documento.Add(child);
                else
                    return null;
            }

            catch (Exception ex)
            {

                return null;
            }

            XDocument doc = new XDocument(Documento);
            return doc;
        }

       
        public static XDocument GeneraXMLAtachado(List<Atached> _listaAtached, IInteraction inx)
        {
            XElement Documento = new XElement("XML");
            

            try
            {
               foreach (var item in _listaAtached)
               {
                    XElement child = new XElement("ATACHED");
                    var valor = inx.GetAttachedData(item.DESCRIPCION) != null ?
                                         inx.GetAttachedData(item.DESCRIPCION).ToString() : null;
                    child.Add(new XElement("OPCION", item.DESCRIPCION));
                    child.Add(new XElement("VALOR", valor));
                    Documento.Add(child);
                }
                
            }

            catch (Exception ex)
            {
             return null;
            }
            XDocument doc = new XDocument(Documento);
            return doc;

        }

     
        public static bool VALIDA_RUT(string rut)
        {
            var validacion = false;
            try
            {
                rut = rut.ToUpper();
                rut = rut.Replace(".", "");
                rut = rut.Replace("-", "");
                var rutAux = int.Parse(rut.Substring(0, rut.Length - 1));

                var dv = char.Parse(rut.Substring(rut.Length - 1, 1));

                int m = 0, s = 1;
                for (; rutAux != 0; rutAux /= 10)
                    s = (s + rutAux % 10 * (9 - m++ % 6)) % 11;
                if (dv == (char)(s != 0 ? s + 47 : 75))
                    validacion = true;
            }
            catch (Exception)
            {
            }
            return validacion;
        }

        public static UIElement GetChildControl(DependencyObject parentObject, string childName)
        {
            UIElement element = null;

            if (parentObject != null)
            {
                var totalChild = VisualTreeHelper.GetChildrenCount(parentObject);
                for (var i = 0; i < totalChild; i++)
                {
                    var childObject = VisualTreeHelper.GetChild(parentObject, i);

                    if (childObject is FrameworkElement && (((FrameworkElement)childObject).Name == childName))
                    {
                        element = childObject as UIElement;
                        break;
                    }

                    // get its child

                    element = GetChildControl(childObject, childName);
                    if (element != null) break;
                }
            }

            return element;
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            var parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            var parent = parentObject as T;
            if (parent != null)
                return parent;
            return FindParent<T>(parentObject);
        }

       

        public static DataTable ListToDataTable<T>(List<T> list)
        {
            var dt = new DataTable();

            try
            {
                foreach (var info in typeof(T).GetProperties())
                    dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
                foreach (var t in list)
                {
                    var row = dt.NewRow();
                    foreach (var info in typeof(T).GetProperties())
                        row[info.Name] = info.GetValue(t, null);
                    dt.Rows.Add(row);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return dt;
        }
    }
}