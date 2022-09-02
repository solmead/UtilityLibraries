using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.ServiceLocator.Interfaces
{
    public interface ILocator
    {
        List<TT> GetServices<TT>() where TT : class;
        TT GetServiceInstance<TT>(Type type) where TT : class;

        TT FindService<TT>(string name) where TT : class, IService;

    }
}
