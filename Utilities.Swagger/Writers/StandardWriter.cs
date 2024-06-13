using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utilities.FileExtensions;
using Utilities.Swagger.Abstract;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Utilities.Swagger.Writers
{
    public class FileEntry
    {
        public string Data { get; set; }
        public FileInfo File { get; set; }
        public string ConfigDirectory { get; set; }
    }
    public abstract class StandardWriter : IFileWriter
    {
        protected IGeneratorInfo _generatorInfo { get; set; } = null;

        public StandardWriter()
        {

        }
        public void SetGenerator(IGeneratorInfo generator)
        {
            _generatorInfo = generator;
        }


        private Dictionary<string, FileEntry> files = new Dictionary<string, FileEntry>();

        public abstract bool NamespacesInOneFile { get; }
        public abstract FileEntry GetModuleFile(string name);
        public abstract FileEntry GetNamespaceFile(string namespaceName);
        public abstract FileEntry GetEnumFile(string name);
        public abstract FileEntry GetModuleIndex();
        public abstract FileEntry GetNamespaceIndex();
        public abstract FileEntry GetEnumIndex();


        public string FindRelativePath(string pathFrom, string pathTo)
        {
            var pt = Path.GetRelativePath(pathFrom, pathTo).Replace("\\","/");

            return pt;
        }

        public void StartFiles()
        {
            var fi = this.GetEnumIndex();
            WriteToFile(fi, false, null, null, false, "//Start of Enum Index" + Environment.NewLine, true);
            fi = this.GetModuleIndex();
            WriteToFile(fi, false, null, null, false, "//Start of Module Index" + Environment.NewLine, true); 
            fi = this.GetNamespaceIndex();
            WriteToFile(fi, false, null, null, false, "//Start of Namespace Index" + Environment.NewLine, true);


        }
        public void FinalizeFiles()
        {
            foreach(var fileItem in files)
            {
                var fi = fileItem.Value.File;
                var file = new System.IO.BinaryWriter(fi.OpenWrite());
                file.Write(System.Text.ASCIIEncoding.ASCII.GetBytes(fileItem.Value.Data));

                file.Close();
            }
        }

        private void WriteToFile(FileEntry fi, bool writeHeader, List<string>? modules, List<string>? enums, bool isRepo, string data, bool isIndex = false)
        {


            if (!files.ContainsKey(fi.File.FullName.ToLower()))
            {
                files.Add(fi.File.FullName.ToLower(), new FileEntry()
                {
                     File = fi.File,
                     Data = "",
                    ConfigDirectory = fi.ConfigDirectory
                });
                if (!isIndex)
                {
                    var indexFile = new FileEntry()
                    {
                        ConfigDirectory = fi.ConfigDirectory,
                        File = new FileInfo(fi.File?.Directory?.FullName + "/index.ts")
                    };

                    WriteToFile(indexFile, false, null, null, false, "export * from './" + fi.File.FileNameWithoutExtension() + "';" + Environment.NewLine, true);
                }
            }


            if (string.IsNullOrEmpty(files[fi.File.FullName.ToLower()].Data))
            {
                isRepo = !isIndex && ( isRepo || NamespacesInOneFile);


                var relativePath = FindRelativePath(fi.ConfigDirectory, "/wwwroot/lib/");

                var head = _generatorInfo.GetFileHeader(modules, enums, relativePath, isRepo);

                files[fi.File.FullName.ToLower()].Data = files[fi.File.FullName.ToLower()].Data + head;
            }

            files[fi.File.FullName.ToLower()].Data = files[fi.File.FullName.ToLower()].Data + data;

            
        }


        public void WrtieStartNameSpaceToFile(string name, List<string> modules, List<string> enums, string? additionalData = null)
        {
            var fi = GetNamespaceFile(name);
            fi.File.Refresh();
            var writeHeader = !fi.File.Exists;

            var str = "";
            if (NamespacesInOneFile)
            {
                str = "     export namespace " + name + " {" + Environment.NewLine;
            }
            str = str + additionalData;

            WriteToFile(fi, writeHeader, modules, enums, true, str);
        }

        public void WrtieEndNameSpaceToFile(string name, string? additionalData = null)
        {
            var fi = GetNamespaceFile(name);
            fi.File.Refresh();
            var writeHeader = !fi.File.Exists;
            var str = "";

            str = str + additionalData;

            if (NamespacesInOneFile)
            {
                str = "     }" + Environment.NewLine;
            }

            WriteToFile(fi, writeHeader, null, null, true, str);
        }



        public void WriteEnumToFile(string name, string enumString)
        {
            var fi = GetEnumFile(name);
            fi.File.Refresh();
            var writeHeader = !fi.File.Exists;
            WriteToFile(fi, writeHeader, null, null, false, enumString);
        }

        public void WriteFunctionToFile(string namespaceString, string name, List<string> modules, List<string> enums, string funcString)
        {
            var fi = GetNamespaceFile(namespaceString);
            fi.File.Refresh();
            var writeHeader = !fi.File.Exists;
            WriteToFile(fi, writeHeader, modules, enums, true, funcString);
        }

        public void WriteModuleToFile(string name, List<string> modules, List<string> enums, string moduleString)
        {
            var fi = GetModuleFile(name);
            fi.File.Refresh();
            var writeHeader = !fi.File.Exists;
            WriteToFile(fi, writeHeader, modules, enums, false, moduleString);
        }


        public abstract string GetModuleToRepoImportLocation(string name);

        public abstract string GetEnumToRepoImportLocation(string name);

        public abstract string GetModuleToModuleImportLocation(string name);

        public abstract string GetEnumToModuleImportLocation(string name);

    }
}
