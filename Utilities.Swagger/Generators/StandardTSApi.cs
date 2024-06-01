using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Swagger.Abstract;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Utilities.Swagger.Generators
{
    public class StandardTSApi : IFileGenerator
    {
        public void EndGroup()
        {
            throw new NotImplementedException();
        }

        public void StartGroup()
        {
            throw new NotImplementedException();
        }

        public void WriteObjectDefinition()
        {
            throw new NotImplementedException();
        }

        public void WriteRemoteCall(string functionString, string paramString, string paramCallString, string funcType, bool hasBody, string bodyName, string origUrl, string finalUrl)
        {
            var data = new StringBuilder();


            data.AppendLine("          export function get" + functionString + "Url(" + paramString + (paramString != "" ? ", " : "") + "options?: any): string {");



            data.AppendLine("               var origUrl = '" + origUrl + "';");
            data.AppendLine("               var url = " + finalUrl + ";");
            data.AppendLine("               var _defaults = { };");
            data.AppendLine("               var _settings = $.extend({ }, _defaults, options || { });");

            data.AppendLine("               for (var key in _settings)");
            data.AppendLine("               {");
            data.AppendLine("                   if (_settings.hasOwnProperty(key) && _settings[key] != null)");
            data.AppendLine("                   {");
            data.AppendLine("                       url += url.indexOf('?') == -1 ? '?' : '&';");
            data.AppendLine("                       url += key + '=' + _settings[key];");
            data.AppendLine("                   }");
            data.AppendLine("               }");
            data.AppendLine("               return url;");
            data.AppendLine("          }");


            data.AppendLine("          export async function " + functionString.ToCamelCasing() + "Async(" + paramString + (paramString != "" ? ", " : "") + "options?: any): Promise<" + funcType + "> {");
            data.AppendLine("               var url = get" + functionString + "Url(" + paramCallString + (paramCallString != "" ? ", " : "") + "options);");
            if (!hasBody)
            {
                data.AppendLine("               var data = null;");
            }

            if (operation.Name == OperationType.Get)
            {
                data.AppendLine("               var value = await ApiLibrary.getCallAsync<" + funcType + ">(url, 0, " + bodyName + ");");
            }
            if (operation.Name == OperationType.Post)
            {
                data.AppendLine("               var value = await ApiLibrary.postCallAsync<" + funcType + ">(url, 0, " + bodyName + ");");
            }
            if (operation.Name == OperationType.Put)
            {
                data.AppendLine("               var value = await ApiLibrary.putCallAsync<" + funcType + ">(url, 0, " + bodyName + ");");
            }
            if (operation.Name == OperationType.Delete)
            {
                data.AppendLine("               var value = await ApiLibrary.deleteCallAsync<" + funcType + ">(url, 0, " + bodyName + ");");
            }
            if (!(funcType == "string" || funcType == "number" || funcType == "boolean"))
            {
                data.AppendLine("");
                data.Append(MapDates(funcType, "value.", "               "));
                data.AppendLine("");
            }
            data.AppendLine("               return value;");
            data.AppendLine("          }");


            var extraName = "";
            if (operation.Name == OperationType.Delete)
            {
                extraName = "Item";
            }


            data.AppendLine("          export async function " + functionString.ToCamelCasing() + extraName + "(" + paramString + (paramString != "" ? ", " : "") + "options?: any, callback?:(data:" + funcType + ")=>void, onError?:(data:Error)=>void): Promise<" + funcType + "> {");
            data.AppendLine("               var url = get" + functionString + "Url(" + paramCallString + (paramCallString != "" ? ", " : "") + "options);");
            if (!hasBody)
            {
                data.AppendLine("               var data = null;");
            }
            data.AppendLine("               try {");
            if (operation.Name == OperationType.Get)
            {
                data.AppendLine("                   var returnData = await ApiLibrary.getCallAsync<" + funcType + ">(url, 0);");
            }
            if (operation.Name == OperationType.Post)
            {
                data.AppendLine("                   var returnData = await ApiLibrary.postCallAsync<" + funcType + ">(url, 0, " + bodyName + ");");
            }
            if (operation.Name == OperationType.Put)
            {
                data.AppendLine("                   var returnData = await ApiLibrary.putCallAsync<" + funcType + ">(url, 0, " + bodyName + ");");
            }
            if (operation.Name == OperationType.Delete)
            {
                data.AppendLine("                   var returnData = await ApiLibrary.deleteCallAsync<" + funcType + ">(url, 0, " + bodyName + ");");
            }
            if (!(funcType == "string" || funcType == "number" || funcType == "boolean"))
            {
                data.AppendLine("");
                data.Append(MapDates(funcType, "returnData.", "                   "));
                data.AppendLine("");
            }
            data.AppendLine("                   if (callback)");
            data.AppendLine("                   {");
            data.AppendLine("                       callback(returnData);");
            data.AppendLine("                   }");
            data.AppendLine("               } catch (error){");
            data.AppendLine("                    let e:Error= error;");
            data.AppendLine("                    if (onError)");
            data.AppendLine("                    {");
            data.AppendLine("                        onError(e);");
            data.AppendLine("                    }");
            data.AppendLine("               }");
            data.AppendLine("               return;");

            data.AppendLine("          }");


        }
    }
}
