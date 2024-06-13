using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Swagger.Abstract
{
    public interface IFileWriter
    {
        bool NamespacesInOneFile { get; }

        public void SetGenerator(IGeneratorInfo generator);

        public void WriteModuleToFile(string name, List<string> modules, List<string> enums, string moduleString);
        public void WriteEnumToFile(string name, string enumString);
        public void WriteFunctionToFile(string namespaceString, string name, List<string> modules, List<string> enums, string enumString);
        public void WrtieStartNameSpaceToFile(string name, List<string> modules, List<string> enums, string? additionalData = null);
        public void WrtieEndNameSpaceToFile(string name, string? additionalData = null);

        public string GetModuleToRepoImportLocation(string name);
        public string GetEnumToRepoImportLocation(string name);
        public string GetModuleToModuleImportLocation(string name);
        public string GetEnumToModuleImportLocation(string name);

        public void StartFiles();
        public void FinalizeFiles();

        string FindRelativePath(string pathFrom, string pathTo);
    }
}
