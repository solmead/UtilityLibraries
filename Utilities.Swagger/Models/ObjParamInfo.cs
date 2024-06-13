using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Swagger.Models
{
    public class ObjParamInfo
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public bool IsNullable { get; set; }

        public bool IsReturned { get; set; } = false;
        public bool IsBody { get; set; } = false;
    }
}
