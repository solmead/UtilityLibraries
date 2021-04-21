using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Utilities.ExcelLibrary
{
    public interface IFileImporter
    {
        DataTable ImportToDataTable<tt>(FileInfo fileInfo, Dictionary<string, string> columnMapping = null) where tt : class;
        DataTable ImportToDataTable<tt>(DirectoryInfo directory, Dictionary<string, string> columnMapping = null) where tt : class;
        DataTable ImportToDataTable<tt>(Stream stream, Dictionary<string, string> columnMapping = null) where tt : class;


        DataTable ImportToDataTable(FileInfo fileInfo);
        DataTable ImportToDataTable(DirectoryInfo directory);
        DataTable ImportToDataTable(Stream stream);


        List<tt> ImportFromFile<tt>(DirectoryInfo directory, Dictionary<string, string> columnMapping, Func<tt> getNewObject, Action<tt, DataTable, DataRow, List<string>> afterLoad = null) where tt : class;

        List<tt> ImportFromFile<tt>(Stream stream, Dictionary<string, string> columnMapping, Func<tt> getNewObject, Action<tt, DataTable, DataRow, List<string>> afterLoad = null) where tt : class;
        List<tt> ImportFromFile<tt>(FileInfo fileInfo, Dictionary<string, string> columnMapping, Func<tt> getNewObject, Action<tt, DataTable, DataRow, List<string>> afterLoad = null) where tt : class;

    }
}
