using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Utilities.ExcelLibrary
{
    public interface IFileExporter
    {

        void Setup(string WorkBookName);
        void AddDataSet(string name, DataTable DT=null);
        void AddDataSet<tt>(string name, IEnumerable<tt> list = null) where tt : class;



        void Save(FileInfo file);
        void Save(DirectoryInfo directory);
        void Save(Stream stream);
    }
}
