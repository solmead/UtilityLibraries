using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Swagger.Filters
{
    public class GenericFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var type = context.Type;

            if (type.IsGenericType == false)
                return;

            //schema.Title = $"{type.Name[0..^2]}<{type.GenericTypeArguments[0].Name}>";
            schema.Title = $"{type.Name[0..^2]}_{type.GenericTypeArguments[0].Name}";
        }
    }
}
