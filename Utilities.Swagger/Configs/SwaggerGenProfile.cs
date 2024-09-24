using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Swagger.Abstract;

namespace Utilities.Swagger.Configs
{


    public abstract class SwaggerGenProfile
    {
        public SwaggerGenProfile() {
            
        }
        public string Name { get; set; }

        //public string LocationToWriteDefinitions { get; set; } = "/wwwroot/js/WebApi/";
        //public string LocationToWriteModules { get; set; } = "/wwwroot/js/WebApi/";
        //public bool CombineToOneFile { get; set; } = false;

        //public string ObjectDefinitionFileName { get; set; } = "";

        internal abstract IFileGenerator GetFileGenerator(ILogger logger, ISwaggerGen swaggerFilterGen);

    }
}
