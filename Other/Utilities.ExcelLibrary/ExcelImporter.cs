using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ClosedXML.Excel;
using Utilities.Poco;

namespace Utilities.ExcelLibrary
{
    public class ExcelImporter
    {

        public static DataTable ImportToDataTable<tt>(FileInfo fileInfo, Dictionary<string, string> columnMapping = null) where tt : class
        {
            var file = new XLWorkbook(fileInfo.FullName);
            var sheet = file.Worksheets.Worksheet(0);
            var tp = typeof(tt);
            var item = (tt)tp.Assembly.CreateInstance(tp.FullName, true);
            IXLRow headerLine = null;
            var headerLineNumber = 0;
            // Dim propList = item.GetPropertyNames()
            var cnt = 1;
            var fnd = false;
            while ((headerLineNumber == 0 && cnt <= sheet.Rows().Count()) && !fnd)
            {
                var line = sheet.Row(cnt); // file.Lines(cnt)
                foreach (var col in line.Cells())
                {
                    if ((item.DoesPropertyExist(col.Value.ToString().Trim()) || (columnMapping != null && columnMapping.ContainsKey(col.Value.ToString().Trim()))))
                    {
                        headerLine = line;
                        headerLineNumber = cnt;
                        fnd = true;
                    }
                }
                cnt += 1;
            }
            Debug.WriteLine("Header Line Number = " + headerLineNumber);

            var table = ToDataTable(sheet, headerLine: headerLineNumber);


            return table;
        }


        public static List<tt> ImportFromFile<tt>(FileInfo fileInfo, Dictionary<string, string> columnMapping, Func<tt> getNewObject, Action<tt, DataTable, DataRow, List<string>> afterLoad = null) where tt : class
        {
            var table = ImportToDataTable<tt>(fileInfo, columnMapping);

            return table.ToList(getNewObject, columnMapping, afterLoad);
        }



        public static DataTable ToDataTable(IXLWorksheet sheet, bool hasHeader = true, int headerLine = 0)
        {
            var maxcolumns = (from l in sheet.Rows()
                from c in l.Cells()
                where !string.IsNullOrWhiteSpace(c.Value.ToString())
                select c.Address.ColumnNumber).Max();
            var maxrows = (from l in sheet.Rows()
                from c in l.Cells()
                where !string.IsNullOrWhiteSpace(c.Value.ToString())
                select c.Address.RowNumber).Max();
            var table = new DataTable();

            for(int cnt=1;cnt<=maxcolumns;cnt++)
            {
                var colName = "";
                if (hasHeader && sheet.Row(headerLine).CellCount() > cnt)
                {
                    colName = sheet.Row(headerLine).Cell(cnt).Value.ToString();
                }
                if (String.IsNullOrEmpty(colName))
                {
                    colName = "Column_" + cnt;
                }

                table.Columns.Add(colName);
            }

            var srow = 1;
            if (hasHeader)
            {
                srow = headerLine + 1;
            }

            for (int cnt = srow; cnt <= maxrows; cnt++)
            {
                var row = table.NewRow();
                for (int cnum = 1; cnum <= maxcolumns; cnum++)
                {
                    var v = "";
                    if (sheet.Row(cnt).CellCount() > cnum)
                    {
                        v = sheet.Row(cnt).Cell(cnum).Value.ToString();
                    }

                    row[cnum - 1] = v;
                }

                table.Rows.Add(row);
            }
            return table;
        }
    }
}
