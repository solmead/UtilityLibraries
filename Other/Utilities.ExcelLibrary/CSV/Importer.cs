
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Utilities.Poco;

namespace Utilities.ExcelLibrary.CSV
{
    public class Importer : IFileImporter
    {
        public string WorkBookname = "";

        public Importer(string name = "Temp")
        {
            WorkBookname = name;
        }

        public List<tt> ImportFromFile<tt>(FileInfo fileInfo, Dictionary<string, string> columnMapping, Func<tt> getNewObject, Action<tt, DataTable, DataRow, List<string>> afterLoad = null) where tt : class
        {
            var table = ImportToDataTable<tt>(fileInfo, columnMapping);

            return table.ToList(getNewObject, columnMapping, afterLoad);
        }

        public List<tt> ImportFromFile<tt>(DirectoryInfo directory, Dictionary<string, string> columnMapping, Func<tt> getNewObject, Action<tt, DataTable, DataRow, List<string>> afterLoad = null) where tt : class
        {
            var file = new FileInfo(directory.FullName + "/" + WorkBookname + ".xlsx");
            return ImportFromFile<tt>(file, columnMapping, getNewObject, afterLoad);
        }

        public List<tt> ImportFromFile<tt>(Stream stream, Dictionary<string, string> columnMapping, Func<tt> getNewObject, Action<tt, DataTable, DataRow, List<string>> afterLoad = null) where tt : class
        {
            var table = ImportToDataTable<tt>(stream, columnMapping);

            return table.ToList(getNewObject, columnMapping, afterLoad);
        }

        public DataTable ImportToDataTable<tt>(FileInfo fileInfo, Dictionary<string, string> columnMapping = null) where tt : class
        {
            DataTable dt = new DataTable();
            //var file = CSVFile.LoadFromFile(fileInfo.FullName);
            using (var reader = new StreamReader(fileInfo.FullName))
            using (var csv = new CsvReader(reader, CultureInfo.CurrentCulture))
            {
                // Do any configuration to `CsvReader` before creating CsvDataReader.
                using (var dr = new CsvDataReader(csv))
                {
                    dt.Load(dr);
                }
            }


            return dt;
        }

        public DataTable ImportToDataTable<tt>(DirectoryInfo directory, Dictionary<string, string> columnMapping = null) where tt : class
        {
            var file = new FileInfo(directory.FullName + "/" + WorkBookname + ".xlsx");
            return ImportToDataTable<tt>(file, columnMapping);
        }

        public DataTable ImportToDataTable<tt>(Stream stream, Dictionary<string, string> columnMapping = null) where tt : class
        {
            DataTable dt = new DataTable();
            //var file = CSVFile.LoadFromFile(fileInfo.FullName);
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, CultureInfo.CurrentCulture))
            {
                // Do any configuration to `CsvReader` before creating CsvDataReader.
                using (var dr = new CsvDataReader(csv))
                {
                    dt.Load(dr);
                }
            }


            return dt;
        }

        public DataTable ImportToDataTable(FileInfo fileInfo)
        {
            DataTable dt = new DataTable();
            //var file = CSVFile.LoadFromFile(fileInfo.FullName);
            using (var reader = new StreamReader(fileInfo.FullName))
            using (var csv = new CsvReader(reader, CultureInfo.CurrentCulture))
            {
                // Do any configuration to `CsvReader` before creating CsvDataReader.
                using (var dr = new CsvDataReader(csv))
                {
                    dt.Load(dr);
                }
            }


            return dt;
        }

        public DataTable ImportToDataTable(DirectoryInfo directory)
        {
            var file = new FileInfo(directory.FullName + "/" + WorkBookname + ".xlsx");
            return ImportToDataTable(file);
        }

        public DataTable ImportToDataTable(Stream stream)
        {
            DataTable dt = new DataTable();
            //var file = CSVFile.LoadFromFile(fileInfo.FullName);
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, CultureInfo.CurrentCulture))
            {
                // Do any configuration to `CsvReader` before creating CsvDataReader.
                using (var dr = new CsvDataReader(csv))
                {
                    dt.Load(dr);
                }
            }


            return dt;
        }
    }
}
