using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq.Dynamic;
using System.Text;

namespace Utilities.ExcelLibrary.Excel
{
    public class Exporter : IFileExporter
    {
        private XLWorkbook workbook;
        public string WorkBookname = "";

        public Exporter(string name = "Temp", DataTable tb = null)
        {
            WorkBookname = name;
            workbook = new XLWorkbook();
            workbook.Properties.Title = this.WorkBookname;
            if (tb != null)
            {
                workbook.Worksheets.Add(tb, name);
            }
        }
        public void AddDataSet(string name, DataTable tb = null)
        {
            if (tb!=null)
            {
                workbook.Worksheets.Add(tb, name);
            } else
            {
                workbook.Worksheets.Add(name);
            }
        }

        public void Save(FileInfo file)
        {
            workbook.SaveAs(file.FullName);
        }

        public void Save(DirectoryInfo directory)
        {
            var file = new FileInfo(directory.FullName + "/" + WorkBookname + ".xlsx");
            workbook.SaveAs(file.FullName);
        }

        public void Save(Stream stream)
        {
            workbook.SaveAs(stream);
        }

        public void Setup(string WorkBookName)
        {
            this.WorkBookname = WorkBookname.Replace(" ", "_");
            workbook.Properties.Title = this.WorkBookname;

        }

        public void AddDataSet<tt>(string name, IEnumerable<tt> list = null) where tt : class
        {

            AddDataSet(name, list.ToDataTable());
        }
    }
}
