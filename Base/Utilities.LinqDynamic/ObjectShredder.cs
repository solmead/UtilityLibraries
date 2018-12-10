using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Utilities.LinqDynamic
{
    public class ObjectShredder<T>
    {
        private FieldInfo[] _fi;
        private PropertyInfo[] _pi;
        private Dictionary<string, int> _ordinalMap;
        private Dictionary<string, bool> _useMap;
        private Dictionary<string, string> _ordinalNameMap;
        private Type _type;

        public ObjectShredder()
        {
            _type = typeof(T);
            _fi = _type.GetFields();
            _pi = _type.GetProperties();
            _ordinalMap = new Dictionary<string, int>();
            _ordinalNameMap = new Dictionary<string, string>();
            _useMap = new Dictionary<string, bool>();
        }


        public DataTable Shred(IEnumerable<T> source, DataTable table, LoadOption? options, bool useDisplayNames = false)
        {

            if (typeof(T).IsPrimitive)
            {
                table = ShredPrimitive(source, table, options);
            }

            if (table == null)
            {
                table = new DataTable(typeof(T).Name);
            }

            // now see if need to extend datatable base on the type T + build ordinal map
            table = ExtendTableBaseClassFirst(table, typeof(T));

            table.BeginLoadData();
            using (IEnumerator<T> e = source.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    if (options != null)
                    {
                        table.LoadDataRow(ShredObject(table, e.Current), options.Value);
                    }
                    else
                    {
                        table.LoadDataRow(ShredObject(table, e.Current), true);
                    }
                }
            }
            table.EndLoadData();
            if ((useDisplayNames))
            {
                RenameColumnsToDisplayName(table);
            }
            return table;
        }




        public DataTable ShredPrimitive(IEnumerable<T> source, DataTable table, LoadOption? options)
        {
            if (table == null)
            {
                table = new DataTable(typeof(T).Name);
            }

            if (!table.Columns.Contains("Value"))
            {
                table.Columns.Add("Value", typeof(T));
            }

            table.BeginLoadData();
            using (IEnumerator<T> e = source.GetEnumerator())
            {
                var values = new object[table.Columns.Count];
                while (e.MoveNext())
                {
                    values[table.Columns["Value"].Ordinal] = e.Current;
                    if ((options != null))
                    {
                        table.LoadDataRow(values, options.Value);
                    }
                    else
                    {
                        table.LoadDataRow(values, true);
                    }
                }
            }
            table.EndLoadData();
            return table;
        }


        public DataTable RenameColumnsToDisplayName(DataTable table)
        {

            foreach (DataColumn col in table.Columns)
            {
                if (_ordinalNameMap.ContainsKey(col.ColumnName))
                {
                    try
                    {
                        col.ColumnName = _ordinalNameMap[col.ColumnName];

                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

            return table;
        }
        public DataTable ExtendTableBaseClassFirst(DataTable table, Type type)
        {
            if ((type.BaseType != null))
            {
                table = ExtendTableBaseClassFirst(table, type.BaseType);
            }

            foreach (FieldInfo f in type.GetFields())
            {
                if ((!_useMap.ContainsKey(f.Name)))
                {
                    DataColumn dc = default(DataColumn);
                    dc = table.Columns.Contains(f.Name) ? table.Columns[f.Name] : table.Columns.Add(f.Name, f.FieldType);
                    _ordinalMap.Add(f.Name, dc.Ordinal);
                    _ordinalNameMap.Add(f.Name, f.Name);
                    _useMap.Add(f.Name, true);
                }
            }

            foreach (PropertyInfo p in type.GetProperties())
            {
                if (!_useMap.ContainsKey(p.Name))
                {
                    Type colType = p.PropertyType;
                    if ((colType.IsGenericType) && (object.ReferenceEquals(colType.GetGenericTypeDefinition(), typeof(Nullable<>))))
                    {
                        colType = colType.GetGenericArguments()[0];
                    }
                    var name = p.Name;
                    var showColumn = true;
                    DisplayNameAttribute attr = p.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault() as DisplayNameAttribute;
                    if ((attr != null))
                    {
                        name = attr.DisplayName;
                    }
                    DisplayAttribute attr2 = p.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault() as DisplayAttribute;
                    if ((attr2 != null))
                    {
                        name = attr2.Name;
                        var sc = attr2.GetAutoGenerateField();
                        showColumn = !sc.HasValue || sc.Value;
                    }


                    if (showColumn)
                    {
                        DataColumn dc = (table.Columns.Contains(p.Name) ? table.Columns[p.Name] : table.Columns.Add(p.Name, colType));
                        _ordinalMap.Add(p.Name, dc.Ordinal);

                        _ordinalNameMap.Add(p.Name, name);
                    }
                    _useMap.Add(p.Name, showColumn);
                }
            }


            return table;
        }

        public object[] ShredObject(DataTable table, T instance)
        {
            FieldInfo[] fi = _fi;
            PropertyInfo[] pi = _pi;

            if (!object.ReferenceEquals(instance.GetType(), typeof(T)))
            {
                ExtendTableBaseClassFirst(table, instance.GetType());
                fi = instance.GetType().GetFields();
                pi = instance.GetType().GetProperties();
            }

            object[] values = new object[table.Columns.Count];
            foreach (FieldInfo f in fi)
            {
                if ((_useMap[f.Name]))
                {
                    values[_ordinalMap[f.Name]] = f.GetValue(instance);
                }
            }

            foreach (PropertyInfo p in pi)
            {
                if ((_useMap[p.Name]))
                {
                    values[_ordinalMap[p.Name]] = p.GetValue(instance, null);
                }
            }
            return values;
        }
    }
}
