using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Swagger.Abstract
{
    public interface ISwaggerGen
    {
        bool IsPrimitive(string dataType);
        string MapDates(string dataType, string classPath = "", string prefillLines = "", int level = 1);
        string DefaultValue(string type);
        bool IsEnum(string type);
        bool IsDate(string type);

        string MapPath(string path);
        bool IsDefinedObject(string dataType);
        bool IsGenericArray(string dataType);
        bool IsArray(string dataType);
        string ArraySubType(string dataType);
    }
}
