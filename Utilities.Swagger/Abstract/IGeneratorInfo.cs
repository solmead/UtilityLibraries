using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Swagger.Abstract
{
    public interface IGeneratorInfo
    {
        string GetFileHeader(List<string> modules, List<string> enums,  string baseLibrariesDirectory, bool isRepo = false);
    }
}
