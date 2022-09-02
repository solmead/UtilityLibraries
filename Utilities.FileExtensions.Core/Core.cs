using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.FileExtensions.AspNetCore
{
    public static class Core
    {
        public static string GetEmbeddedBasePath(Assembly assembly )
        {

            var nm = assembly.GetName().Name.Split(".").Last();

            return "/" + nm; // "/Shares/" + assembly.GetName().Name;
        }

    }
}
