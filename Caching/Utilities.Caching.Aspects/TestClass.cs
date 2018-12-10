using System.Collections.Generic;
using System.Threading.Tasks;

namespace Utilities.Caching.Aspects
{
    public class TestClass
    {
        private List<int> _myList = new List<int>() {5, 4, 3, 2, 1};
        private List<int> _myList2 = new List<int>() { 55, 44, 33, 22, 11 };

        [CacheProperty(CacheArea = CacheArea.Global)]
        public List<int> MyList1
        {
            get
            {
                return _myList;
            }
            set
            {
                _myList = value;
            }
        }


        [CacheProperty(CacheArea = CacheArea.Global)]
        public List<int> MyList2
        {
            get
            {
                return _myList2;
            }
            private set
            {
                _myList2 = value;
            }
        }

        public void SetMyList(List<int> lst)
        {
            MyList2 = lst;
        }


        [CacheFunction(CacheArea = CacheArea.Global)]
        public static async Task<List<int>> DoSomething(int a = 0, string b = "test")
        {
            return new List<int>() {5,4,3,2,1};
        }


        [CacheFunction(CacheArea = CacheArea.Global)]
        public static List<int> DoSomething2(int a = 0, string b = "test")
        {
            return new List<int>() {6,7,8,9,10, a};
        }

        [CacheFunction(CacheArea = CacheArea.Global)]
        public static List<int> DoSomething3()
        {
            return new List<int>() { 16, 17, 18, 19, 10 };
        }

        [CacheFunction(CacheArea = CacheArea.Request, AspectPriority = 1)]
        [CacheFunction(CacheArea = CacheArea.Global, AspectPriority = 2)]
        public static List<int> DoSomething4(string name = "test")
        {
            return new List<int>() { 161, 171, 181, 191, 101 };
        }


        public static void SetValues(List<int> val)
        {
            CacheAspects.SetOnFunction(DoSomething3, val);
        }
    }
}
