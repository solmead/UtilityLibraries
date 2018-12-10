using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Extensions
{
    public class DeferredList<tt> :IList<tt> where tt:class
    {
        public delegate List<tt> RequestFunction();
        public delegate IQueryable<tt> QueryableFunction();

        private List<tt> _list = null;
        private RequestFunction request;
        public DeferredList(RequestFunction requester)
        {
            request = requester;
        }
        public DeferredList(QueryableFunction requester)
        {
            request = ()=> requester().ToList();
        }

        private List<tt> inner
        {
            get
            {
                if (_list == null)
                {
                    _list = request();
                }
                return _list;
            }
        }


        public IEnumerator<tt> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(tt item)
        {
            inner.Add(item);
        }

        public void Clear()
        {
            inner.Clear();
        }

        public bool Contains(tt item)
        {
            return inner.Contains(item);
        }

        public void CopyTo(tt[] array, int arrayIndex)
        {
            foreach (var itm in inner)
            {
                array[arrayIndex] = itm;
                arrayIndex++;
            }
        }

        public bool Remove(tt item)
        {
            return inner.Remove(item);
        }

        public int Count { get { return inner.Count; } }
        public bool IsReadOnly { get { return true; }}
        public int IndexOf(tt item)
        {
            return inner.IndexOf(item);
        }

        public void Insert(int index, tt item)
        {
            inner.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            inner.RemoveAt(index);
        }

        public tt this[int index]
        {
            get { return inner[index]; }
            set { inner[index] = value; }
        }
    }
}
