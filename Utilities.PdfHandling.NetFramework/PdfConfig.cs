namespace Utilities.PdfHandling.NetFramework
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
        public ServerEnum CurrentServer { get; set; } = ServerEnum.Development;

    }
}
