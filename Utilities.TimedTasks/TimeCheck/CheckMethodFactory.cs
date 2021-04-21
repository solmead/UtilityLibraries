using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.TimedTasks.TimeCheck
{
    internal static class CheckMethodFactory
    {
        private static List<ICheck> checkList = new List<ICheck>();
        private static ICheck Create(Type tp)
        {
            var newItem = (ICheck)tp.Assembly.CreateInstance(tp.FullName);
            return newItem;
        }

        private static List<ICheck> GetCheckHandlers()
        {

            if (!checkList.Any())
            {
                
                    try
                    {
                        var types = Core.getAssemblyTypes();
                        var lst = (from t in types
                                   where t.GetInterfaces().Contains(typeof(ICheck))
                                   select (ICheck)Create(t)).ToList();
                        checkList.AddRange(lst);
                    }
                    catch //(Exception ex2)
                    {
                        //_logger.LogError(ex2, ex2.Message);
                        //var i = 0;
                    }
                
            }
            return checkList;
        }




        public static ICheck GetCheckMethod(CallRateEnum callRate)
        {

            var it = GetCheckHandlers().Where((c) => c.CallRate == callRate).FirstOrDefault();

            return it;

        }
    }
}
