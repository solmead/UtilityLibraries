using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Swagger.Abstract;
using Utilities.Swagger.Generators;
using Utilities.Swagger.Writers;

namespace Utilities.Swagger.Configs
{
    public class SwaggerStandardConfig : SwaggerGenProfile
    {

        public string Path = "/wwwroot/js/WebApi/";
        public string Filename = "Api.ts";

        public SwaggerStandardConfig() :base()
        {
            Name = "StandardApi";
        }

        internal override IFileGenerator GetFileGenerator(ILogger logger, ISwaggerGen swaggerFilterGen)
        {
            return new StandardTSGenerator(this, logger, swaggerFilterGen, new StandardSingleFileWriter(this, logger, swaggerFilterGen));
        }
    }
}
