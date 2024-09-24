using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Swagger.Models
{
    public class ObjDefInfo
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public Dictionary<string, ObjParamInfo> Params { get; set; } = new Dictionary<string, ObjParamInfo>();
    }
    public class ObjParamInfo
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public bool IsNullable { get; set; }

        public bool IsReturned { get; set; } = false;
        public bool IsBody { get; set; } = false;
    }
}
