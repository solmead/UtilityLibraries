using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Swagger.Abstract
{
    public interface IFileGenerator
    {


        void StartGroup();
        void EndGroup();

        void WriteRemoteCall(string functionString, string paramString, string paramCallString, string funcType, bool hasBody, string bodyName, string origUrl, string finalUrl);

        void WriteObjectDefinition();


    }
}
