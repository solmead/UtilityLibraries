using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Extensions
{
    public class DeferredDictionary<tKey, tValue> : IDictionary<tKey, tValue> where tValue : class
    {
        public delegate Dictionary<tKey, tValue> RequestFunction();

        private Dictionary<tKey, tValue> _dic = null;
        private RequestFunction request;
        public DeferredDictionary(RequestFunction requester)
        {
            request = requester;
        }

        private Dictionary<tKey, tValue> inner
        {
            get
            {
                if (_dic == null)
                {
                    _dic = request();
                }
                return _dic;
            }
        }



        public IEnumerator<KeyValuePair<tKey, tValue>> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<tKey, tValue> item)
        {
            inner.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            inner.Clear();
        }

        public bool Contains(KeyValuePair<tKey, tValue> item)
        {
            return inner.Contains(item);
        }

        public void CopyTo(KeyValuePair<tKey, tValue>[] array, int arrayIndex)
        {
            foreach (var key in inner.Keys)
            {
                array[arrayIndex] = new KeyValuePair<tKey, tValue>(key, inner[key]);
                arrayIndex++;
            }
        }

        public bool Remove(KeyValuePair<tKey, tValue> item)
        {
            return inner.Remove(item.Key);
        }

        public int Count { get { return inner.Count; } }
        public bool IsReadOnly
        {
            get { return true; }
        }
        public bool ContainsKey(tKey key)
        {
            return inner.ContainsKey(key);
        }

        public void Add(tKey key, tValue value)
        {
            inner.Add(key, value);
        }

        public bool Remove(tKey key)
        {
            return inner.Remove(key);
        }

        public bool TryGetValue(tKey key, out tValue value)
        {
            value = null;
            if (inner.ContainsKey(key))
            {
                value = inner[key];
                return true;
            }
            return false;
        }

        public tValue this[tKey key]
        {
            get { return inner[key]; }
            set { inner[key] = value; }
        }

        public ICollection<tKey> Keys
        {
            get { return inner.Keys; }
        }
        public ICollection<tValue> Values { get { return inner.Values; } }
    }
}
