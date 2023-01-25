using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.FileExtensions.AspNetCore
{




    public interface IPhysicalFileServerProvider
    {
        /// <summary>
        /// Contains a list of FileServer options, a combination of virtual + physical paths we can access at any time
        /// </summary>
        IList<FileServerOptions> FileServerOptionsCollection { get; }

        /// <summary>
        /// Gets the IFileProvider to access a physical location by using its virtual path
        /// </summary>
        IFileProvider GetProvider(string virtualPath);
    }
    public interface IEmbeddedFileServerProvider : IPhysicalFileServerProvider
    {

    }

    /// <summary>
    /// Implements IFileServerProvider in a very simple way, for demonstration only
    /// </summary>
    public class EmbeddedFileServerProvider : IEmbeddedFileServerProvider
    {
        public EmbeddedFileServerProvider(IList<FileServerOptions> fileServerOptions)
        {
            FileServerOptionsCollection = fileServerOptions;
        }

        public IList<FileServerOptions> FileServerOptionsCollection { get; }

        public IFileProvider GetProvider(string virtualPath)
        {
            try
            {
                var options = FileServerOptionsCollection.FirstOrDefault(e => e.RequestPath == virtualPath);
                if (options == null)
                    throw new FileNotFoundException($"virtual path {virtualPath} is not registered in the fileserver provider");

                return options.FileProvider;

            }
            catch
            {
                return null;
            }

        }
    }

    /// <summary>
    /// Implements IFileServerProvider in a very simple way, for demonstration only
    /// </summary>
    public class PhysicalFileServerProvider : IPhysicalFileServerProvider
    {
        public PhysicalFileServerProvider(IList<FileServerOptions> fileServerOptions)
        {
            FileServerOptionsCollection = fileServerOptions;
        }

        public IList<FileServerOptions> FileServerOptionsCollection { get; }

        public IFileProvider GetProvider(string virtualPath)
        {
            try
            {
                var options = FileServerOptionsCollection.FirstOrDefault(e => e.RequestPath == virtualPath);
                //if (options == null)
                //    throw new FileNotFoundException($"virtual path {virtualPath} is not registered in the fileserver provider");

                return options?.FileProvider;

            }
            catch
            {
                return null;
            }

        }
    }
}
