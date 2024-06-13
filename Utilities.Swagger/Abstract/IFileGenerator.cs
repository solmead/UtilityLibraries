using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Swagger.Models;

namespace Utilities.Swagger.Abstract
{
    public interface IFileGenerator
    {
        void StartSwaggerGen();
        void EndSwaggerGen();

        void StartComponents(string componentName);
        void EndComponents(string componentName);
        void StartSchemas(string schemaName);
        void EndSchemas(string schemaName);

        void StartNamespaces();
        void EndNamespaces();

        void StartNamespace(string namespaceName, Dictionary<string, ObjParamInfo> objectParams);
        void EndNamespace(string namespaceName);

        void WriteRemoteCall(string namespaceName, OperationType operation, string functionString, Dictionary<string, ObjParamInfo> objectParams, string url, string? messages = null);

        void WriteObjectDefinition(string className, Dictionary<string, ObjParamInfo> objectParams);

        void WriteEnumDefinition(string enumName, List<string> values);



    }
}
