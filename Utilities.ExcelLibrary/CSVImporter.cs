using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using Utilities.Poco;

namespace Utilities.ExcelLibrary
{
    public class CSVImporter
    {
        public static DataTable ImportToDataTable<tt>(FileInfo fileInfo, Dictionary<string, string> columnMapping = null) where tt : class
        {
            var file = CSVFile.LoadFromFile(fileInfo.FullName);
            var tp = typeof(tt);
            var item = (tt)tp.Assembly.CreateInstance(tp.FullName, true);
            CSVFile.CSVLine headerLine = null/* TODO Change to default(_) if this is not a reference type */;
            var headerLineNumber = 0;
            // Dim propList = item.GetPropertyNames()
            var cnt = 0;
            var fnd = false;
            while ((headerLineNumber == 0 && cnt < file.Lines.Count) && !fnd)
            {
                var line = file.Lines[cnt];
                foreach (var col in line.Columns)
                {
                    if ((item.DoesPropertyExist(col.Trim()) || (columnMapping != null && columnMapping.ContainsKey(col.Trim()))))
                    {
                        headerLine = line;
                        headerLineNumber = cnt;
                        fnd = true;
                    }
                }
                cnt += 1;
            }
            Debug.WriteLine("Header Line Number = " + headerLineNumber);
            var table = file.ToDataTable(headerLine: headerLineNumber);


            return table;
        }


        public static List<tt> ImportFromFile<tt>(FileInfo fileInfo, Dictionary<string, string> columnMapping, Func<tt> getNewObject, Action<tt, DataTable, DataRow, List<string>> afterLoad = null) where tt : class
        {
            var table = ImportToDataTable<tt>(fileInfo, columnMapping);

            return table.ToList(getNewObject, columnMapping, afterLoad);
        }
    }
}
