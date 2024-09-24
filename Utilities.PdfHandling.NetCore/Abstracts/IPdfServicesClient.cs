using FluentResults;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities.PdfHandling.Models;

namespace Utilities.PdfHandling.NetCore.Abstracts
{
    public interface IPdfServicesClient
    {

        Task<Result<FileEntry>> GetPdfFromUrlAsync(string url, PageOrientation orientation = PageOrientation.Portrait);


        Result<FileEntry> GetPdfFromUrl(string url, PageOrientation orientation = PageOrientation.Portrait);
    }
}
