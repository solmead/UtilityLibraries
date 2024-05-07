using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.PdfHandling
{

    public enum ServerEnum
    {
        Development,
        QA,
        Scan,
        Production
    }

    public class PdfConfig
    {
        public string ConnectionString { get; set; } = "http://uc-activepdf.northcentralus.cloudapp.azure.com/pdfconvert.svc";
       

        public ServerEnum CurrentServer { get; set; } = ServerEnum.Development;



    }
}
