using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.FileExtensions.Services
{
    public interface IEmbeddedFileHandling<TT> : IFileHandling
        where TT:class
    {

    }
}
