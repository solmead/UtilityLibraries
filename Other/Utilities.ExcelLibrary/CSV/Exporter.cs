using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;

namespace Utilities.ExcelLibrary.CSV
{
    public class Exporter : IFileExporter
    {

        public string WorkBookname = "";
        Dictionary<string, DataTable> sheets = new Dictionary<string, DataTable>();

        public bool useWindowsLines { get; set; } = true;

        public Exporter(string name = "Temp", DataTable tb = null)
        {
            WorkBookname = name;
            if (tb != null)
            {
                sheets.Add(name, tb);
            }
        }
        

        public void AddDataSet(string name, DataTable DT = null)
        {
            sheets.Add(name, DT);
        }
        public void AddDataSet<tt>(string name, IEnumerable<tt> list = null) where tt : class
        {

            AddDataSet(name, list.ToDataTable());
        }

        public void Save(FileInfo file)
        {
            var ms = new MemoryStream();
            Save(ms);
            var s = file.OpenWrite();
            s.Write(ms.ToArray(), 0, (int)ms.Length);
            s.Close();
        }

        public void Save(DirectoryInfo directory)
        {
            var file = new FileInfo(directory.FullName + "/" + WorkBookname + ".xlsx");
            Save(file);
        }

        public void Save(Stream stream)
        {
            var sb = new StringBuilder();
            var dt = sheets.Values.FirstOrDefault();
            using (var textWriter = new StringWriter(sb))
            using (var csv = new CsvWriter(textWriter, CultureInfo.CurrentCulture))
            {
                textWriter.NewLine = (useWindowsLines ? "\r\n" : "\r");
                // Write columns
                foreach (DataColumn column in dt.Columns)
                {
                    csv.WriteField(column.ColumnName);
                }
                csv.NextRecord();

                // Write row values
                foreach (DataRow row in dt.Rows)
                {
                    for (var i = 0; i < dt.Columns.Count; i++)
                    {
                        csv.WriteField(row[i]);
                    }
                    csv.NextRecord();
                }
                //csv.NextRecord();
            }

            using (var s = new System.IO.StreamWriter(stream))
            {
                var st = sb.ToString();
                if (!useWindowsLines)
                {
                    st = st.Replace("\n", "");
                }
                s.Write(st);
                s.Flush();
            }
        }

        public void Setup(string WorkBookName)
        {
            this.WorkBookname = WorkBookname.Replace(" ", "_");
        }
    }
}
