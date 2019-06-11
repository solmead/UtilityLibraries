using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections;
using System.Data;

namespace Utilities.Poco
{
    public static class Extensions
    {

        private static Dictionary<string, Dictionary<String, String>> mappings = new Dictionary<string, Dictionary<String, String>>();



        public static TItem ToItem<TItem>(this DataRow row, DataTable table, Func<TItem> getNewObject = null, Dictionary<string, string> columnMapping = null, Action<TItem, DataTable, DataRow, List<string>> afterLoad = null, Dictionary<string, string> mappings = null) where TItem : class
        {

            if (getNewObject == null)
            {
                getNewObject = () =>
                {
                    Type tp = typeof(TItem);
                    TItem newItem = (TItem)tp.Assembly.CreateInstance(tp.FullName);
                    return newItem;
                };
            }
            if ((mappings == null))
            {
                mappings = GetDefinedMappings(getNewObject());
            }
            if (columnMapping == null)
            {
                columnMapping = new Dictionary<string, string>();
            }

            foreach (var key in columnMapping.Keys)
            {
                if ((!mappings.ContainsKey(key)))
                {
                    mappings.Add(key, columnMapping[key]);
                }
                else
                {
                    mappings[key] = columnMapping[key];
                }
            }

            var colNames = (from DataColumn c in table.Columns select c.ColumnName).ToList();

            var item = getNewObject();
            for (var col = 0; col <= table.Columns.Count - 1; col++)
            {
                try
                {
                    var origColumnName = table.Columns[col].ColumnName.Trim();

                    if (!row.IsNull(col))
                    {
                        var column = row[col];
                        var newColumnName = origColumnName;
                        if ((mappings.ContainsKey(origColumnName)))
                        {
                            newColumnName = mappings[origColumnName];
                        }
                        if ((item.DoesPropertyExist(newColumnName, mappings)))
                        {
                            colNames.Remove(origColumnName);
                            item.SetPropertyOn(newColumnName, column, mappings);
                        }
                    }
                    else
                    {
                        var newColumnName = origColumnName;
                        if ((mappings.ContainsKey(origColumnName)))
                        {
                            newColumnName = mappings[origColumnName];
                        }
                        if ((item.DoesPropertyExist(newColumnName, mappings)))
                        {
                            colNames.Remove(origColumnName);
                            item.SetPropertyOn(newColumnName, null, mappings);
                        }
                    }

                }
                catch (Exception ex)
                {
                    var i = 0;
                }
            }

            if ((afterLoad != null))
            {
                afterLoad(item, table, row, colNames);
            }


            return item;
        }


        public static List<TItem> ToList<TItem>(this DataTable table, Func<TItem> getNewObject = null, Dictionary<string, string> columnMapping = null, Action<TItem, DataTable, DataRow, List<string>> afterLoad = null) where TItem : class
        {
            if (getNewObject == null)
            {
                getNewObject = () =>
                {
                    Type tp = typeof(TItem);
                    TItem newItem = (TItem)tp.Assembly.CreateInstance(tp.FullName);
                    return newItem;
                };
            }

            var mappings = GetDefinedMappings(getNewObject());
            if (columnMapping == null)
            {
                columnMapping = new Dictionary<string, string>();
            }

            foreach (var key in columnMapping.Keys)
            {
                if ((!mappings.ContainsKey(key)))
                {
                    mappings.Add(key, columnMapping[key]);
                }
                else
                {
                    mappings[key] = columnMapping[key];
                }
            }

            var itemList = new List<TItem>();
            foreach (DataRow row in table.Rows)
            {
                var item = row.ToItem(table, getNewObject, null, afterLoad, mappings);
                itemList.Add(item);
            }
            return itemList;


        }





        private static void SetPropertyOn<TItem>(this TItem item, string name, object value, Dictionary<string, string> mappings = null) where TItem : class
        {


            if ((name.Contains(".")))
            {
                var tstr = name.Split('.');
                Type tp = item.GetPropertyType(tstr[0], mappings);
                var pitem = item.GetValue(tstr[0], mappings);
                if (pitem == null)
                {
                    pitem = tp.Assembly.CreateInstance(tp.FullName);
                    item.SetValue(tstr[0], pitem);
                }


                if (pitem is IEnumerable)
                {
                }
                try
                {
                    SetPropertyOn<object>(pitem, name.Replace(tstr[0] + ".", ""), value, mappings);
                }
                catch (Exception ex)
                {
                    var i = 0;
                }
            }
            else
            {
                item.SetPropOnObj(name, value, mappings);
            }
        }


        public static bool IsNullableType(Type myType)
        {
            return (myType.IsGenericType) && (myType.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static void SetPropOnObj<TItem>(this TItem item, string propertyName, object value, Dictionary<string, string> mappings = null)
        {
            var headCol = propertyName;
            var column = value;


            if ((item.DoesPropertyExist(headCol, mappings)))
            {
                var tpe = item.GetPropertyType(headCol, mappings);
                try
                {
                    if (column == null && IsNullableType(tpe))
                    {
                        //Dim v = Convert.ChangeType(column, tpe)
                        item.SetValue(headCol, column, mappings);
                        return;
                    }

                }
                catch (Exception ex)
                {
                }
                var isNullable = IsNullableType(tpe);
                var bTpe = tpe;
                if (isNullable)
                {
                    tpe = Nullable.GetUnderlyingType(tpe);
                }

                var IsEnum = tpe.IsEnum;

                var tName = tpe.FullName.ToUpper();
                if ((tName.Contains("DATETIME")))
                {
                    System.DateTime v = default(System.DateTime);
                    if (column == null)
                    {
                        column = "";
                    }

                    DateTime.TryParse(column.ToString(),out v);
                    item.SetValue(headCol, v, mappings);
                }
                else if ((tName.Contains("BOOL")))
                {
                    if (column == null)
                    {
                        column = "";
                    }
                    column = column.ToString().ToUpper().Replace("YES", "TRUE").Replace("NO", "FALSE").Replace("0", "FALSE").Replace("N", "FALSE").Replace("Y", "TRUE").Replace("1", "TRUE");
                    bool v = false;
                    bool.TryParse(column.ToString(),out v);
                    item.SetValue(headCol, v, mappings);
                }
                else if ((tName.Contains("INT")))
                {
                    if (column == null)
                    {
                        column = "";
                    }
                    column = column.ToString().Replace("$", "").Replace(",", "");
                    int v = 0;
                    int.TryParse(column.ToString(),out v);
                    item.SetValue(headCol, v, mappings);
                }
                else if ((tName.Contains("FLOAT")))
                {
                    if (column == null)
                    {
                        column = "";
                    }
                    column = column.ToString().Replace("$", "").Replace(",", "");
                    float v = 0;
                    float.TryParse(column.ToString(),out v);
                    item.SetValue(headCol, v, mappings);
                }
                else if ((tName.Contains("DOUBLE")))
                {
                    if (column == null)
                    {
                        column = "";
                    }
                    column = column.ToString().Replace("$", "").Replace(",", "");
                    double v = 0;
                    double.TryParse(column.ToString(),out v);
                    item.SetValue(headCol, v, mappings);
                }
                else if ((tName.Contains("LONG")))
                {
                    if (column == null)
                    {
                        column = "";
                    }
                    column = column.ToString().Replace("$", "").Replace(",", "");
                    long v = 0;
                    long.TryParse(column.ToString(),out v);
                    item.SetValue(headCol, v, mappings);
                }
                else if ((tName.Contains("DECIMAL")))
                {
                    if (column == null)
                    {
                        column = "";
                    }
                    column = column.ToString().Replace("$", "").Replace(",", "");
                    decimal v = default(decimal);
                    decimal.TryParse(column.ToString(),out v);
                    item.SetValue(headCol, v, mappings);
                }
                else
                {
                    if (!IsEnum)
                    {
                        var v = Convert.ChangeType(column, tpe);
                        item.SetValue(headCol, v);
                    }
                    else
                    {
                        if (column == null)
                        {
                            column = "";
                        }
                        try
                        {
                            var exists = (from t in Enum.GetNames(tpe) where t.ToUpper() == column.ToString().Trim().ToUpper() select t).Any();
                            if ((exists))
                            {
                                var v = Enum.Parse(tpe, column.ToString());
                                item.SetValue(headCol, v, mappings);
                            }
                            else
                            {
                                int num = 0;
                                int.TryParse(column.ToString(), out num);
                                item.SetValue(headCol, num, mappings);
                            }
                        }
                        catch (Exception ex)
                        {
                            var i = 0;
                        }
                    }
                }




            }
            else if ((!string.IsNullOrEmpty(headCol)))
            {
                //Throw New Exception("Column Not Handled: [" + headCol + "]")
            }
        }


        public static TItem Create<TItem>()
        {
            var tp = typeof(TItem);
            var newItem = (TItem)tp.Assembly.CreateInstance(tp.FullName);
            return newItem;
        }



        public static TItem Clone<TItem>(this TItem item)
        {
            var newItem = Create<TItem>();
            item.CopyInto(newItem);
            return newItem;
        }
        public static void CloneFrom<TItem1, TItem2>(this TItem1 item, TItem2 oldItem)
        {
            oldItem.CopyInto(item);
        }
        public static void CopyInto<TItem1, TItem2>(this TItem1 item, TItem2 newItem)
        {
            if (newItem==null)
            {
                throw new Exception("Must be copied into valid object");
            }
            foreach(var p in item.GetPropertyNames(onlyBaseTypes: true, onlyWritable: true))
            {
                newItem.SetValue(p, item.GetValue(p));
            }
        }


        public static NameValueCollection PropertiesAsCollection<TItem>(this TItem item)
        {
            var f = new NameValueCollection();
            foreach (var p in item.GetPropertyNames(onlyBaseTypes: true, onlyWritable: false))
            {
                var v = item.GetValue(p);
                if (v != null)
                {
                    v = v.ToString();
                }
                f[p]= (string)v;
            }
            return f;

        }

        public static Dictionary<string, string> PropertiesAsDictionary<TItem>(this TItem item)
        {
            var f = new Dictionary<string, string>();
            foreach(var p in item.GetPropertyNames(onlyBaseTypes: true, onlyWritable: false))
            {
                var v = item.GetValue(p);
                if (v!=null)
                {
                    v = v.ToString();
                }
                f.Add(p, (string)v);
            }
            return f;

        }

        public static PropertyInfo GetProperty<TItem>(this TItem item, string propertyName, Dictionary<string, string> mappings = null)
        {
            var pName = propertyName;
            if (mappings == null)
            {
                mappings = GetDefinedMappings(item);
            }
            if (mappings.ContainsKey(propertyName))
            {
                pName = mappings[propertyName];
            }

            var tp = item.GetType();
            var prop = tp.GetProperty(pName);
            return prop;
        }

        public static Type GetPropertyType<TItem>(this TItem item, string propertyName, Dictionary<string, string> mappings = null)
        {
            var prop = item.GetProperty(propertyName, mappings);
            return prop?.PropertyType ?? typeof(string);
        }

        public static void SetValue<TItem>(this TItem item, string propertyName, object value, Dictionary<string, string> mappings = null)
        {
            var prop = item.GetProperty(propertyName, mappings);
            if (prop?.CanWrite ?? false)
            {
                prop.SetValue(item, value);
            }
        }

        public static Object GetValue<TItem>(this TItem item, string propertyName, Dictionary<string, string> mappings = null)
        {
            var prop = item.GetProperty(propertyName, mappings);
            if (prop?.CanRead ?? false)
            {
                return prop.GetValue(item);
            }
            return null;
        }




        public static bool DoesPropertyExist<TItem>(this TItem item, string propertyName, Dictionary<string, string> mappings = null)
        {
            var pName = propertyName;
            if ((mappings == null))
            {
                mappings = GetDefinedMappings(item);
            }

            if (mappings.ContainsKey(propertyName))
            {
                pName = mappings[propertyName];
            }

            if (pName.Contains("."))
            {
                var tstr = pName.Split('.');
                var prop = item.GetProperty(tstr[0], mappings);
                if (prop != null) { 
                    Type tp = item.GetPropertyType(tstr[0], mappings);
                    object pitem = item.GetValue(tstr[0], mappings);
                    if ((pitem == null))
                    {
                        pitem = tp.Assembly.CreateInstance(tp.FullName);
                    }

                    return DoesPropertyExist(pitem, propertyName.Replace((tstr[0] + "."), ""));
                }

                return false;
            }
            else
            {
                var prop = item.GetProperty(propertyName, mappings);
                return prop != null;
            }
            
        }

        public static List<String> GetPropertyNames<TItem>(this TItem item, bool onlyWritable = true, bool onlyBaseTypes = false)
        {
            Type tp = item.GetType();

            var props = tp.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy).ToList();

            if (onlyWritable)
            {
                props = (from p in props where p.CanWrite select p).ToList();
            }

            return (from p in props select p.Name).ToList();
        }
        
        public static string GetPropertyAlternateName<TItem>(this TItem item, string propertyName, Dictionary<string, string> mappings = null) where TItem : class
        {
            var itm = item.GetProperty(propertyName, mappings);
            IEnumerable<object> attrs = (from dynamic ca in itm.GetCustomAttributes(true) where ca.TypeId.FullName.Contains("ColumnAttribute") select ca).ToList();
            

            DisplayAttribute attr2 = itm.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault() as DisplayAttribute;
            if ((attr2 != null))
            {
                return attr2.Name;
            }

            DisplayNameAttribute attr3 = itm.GetCustomAttributes(typeof(DisplayNameAttribute), true).FirstOrDefault() as DisplayNameAttribute;
            if ((attr3 != null))
            {
                return attr3.DisplayName;
            }
            DescriptionAttribute attr4 = itm.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault() as DescriptionAttribute;
            if ((attr4 != null))
            {
                return attr4.Description;
            }

            foreach (dynamic attr in attrs)
            {
                try
                {
                    if ((attr != null) && !string.IsNullOrWhiteSpace(attr.Name))
                    {
                        return attr.Name;
                    }

                }
                catch (Exception ex)
                {
                }
            }
            return "";
        }

        private static Dictionary<string, string> GetDefinedMappings<TItem>(TItem item, bool onlyWritable  = true, bool onlyBaseTypes = false)
        {
            var tp = item.GetType();

            var name = tp.ToString();

            Dictionary<String, String> dic = null;

            try
            {
                if (mappings.ContainsKey(name.ToUpper()))
                {
                    dic = mappings[name.ToUpper()];
                }
            }
            catch
            {

            }
            

            if (dic == null)
            {
                dic = new Dictionary<String, String>();
                if (!mappings.ContainsKey(name.ToUpper()))
                {
                    mappings.Add(name.ToUpper(), dic);
                }

                

                var props = tp.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy).ToList();

                if (onlyWritable)
                {
                    props = (from p in props where p.CanWrite select p).ToList();
                }

                var lst = (from p in props
                           where (from dynamic ca in p.GetCustomAttributes(true)
                                                  where ca.TypeId.FullName.Contains("ColumnAttribute")                          select ca).Any() || 
                                p.GetCustomAttributes(typeof(DisplayAttribute), true).Any() || 
                                p.GetCustomAttributes(typeof(DisplayNameAttribute), true).Any()
                           select p).ToList();

                foreach (var itm in lst)
                {
                    var attrs = (from ca in itm.GetCustomAttributes(true) where ca.GetType().FullName.Contains("ColumnAttribute") select ca).ToList();
                    foreach (dynamic attr in attrs)
                    {
                        try
                        {

                            //var property = attr.GetType().GetProperty("Name");
                            //var name = (string)property.GetValue(attr, null);

                            if ((attr != null) && !String.IsNullOrWhiteSpace(attr.Name))
                            {
                                dic.Add(attr.Name, itm.Name);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    var attr2 = itm.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault() as DisplayAttribute;
                    if ((attr2 != null) && !dic.ContainsKey(attr2.Name))
                    {
                        dic.Add(attr2.Name, itm.Name);
                    }

                    var attr3 = itm.GetCustomAttributes(typeof(DisplayNameAttribute), true).FirstOrDefault() as DisplayNameAttribute;
                    if ((attr3 != null) && !dic.ContainsKey(attr3.DisplayName))
                    {
                        dic.Add(attr3.DisplayName, itm.Name);
                    }


                    var attr4 = itm.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault() as DescriptionAttribute;
                    if ((attr4 != null) && !dic.ContainsKey(attr4.Description))
                    {
                        dic.Add(attr4.Description, itm.Name);
                    }

                }
            }
            return dic;
        }

    }
}
