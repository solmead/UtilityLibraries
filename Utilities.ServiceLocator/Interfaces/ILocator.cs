using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.ServiceLocator.Interfaces
{
    public interface ILocator
    {
        [Obsolete("Do not use this, instead use the FindService call", true)]
        List<TT> GetServices<TT>() where TT : class;
        TT GetServiceInstance<TT>(Type type) where TT : class;

        TT FindService<TT>(string name) where TT : class, IService;
        TT FindService<TT>(Func<TT, bool> whereClause) where TT : class, IService;

        void ExecutePerService<TT>(Action<TT> actionClause, Func<TT, int> orderBy = null) where TT : class, IService;

    }
}
