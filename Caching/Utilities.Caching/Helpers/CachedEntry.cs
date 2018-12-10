using System;
using System.Xml.Serialization;

namespace Utilities.Caching.Helpers
{
    public interface ICacheEntry
    {
        string GetDataString();
        object ItemObject { get; }
    }
    [Serializable]
    public abstract class CachedEntryBase
    {
        protected abstract ICacheEntry GetMe();
        public string Name { get; set; }
        public string Data { get { return GetMe().GetDataString(); } }
        public DateTime Created { get; set; }
        public DateTime Changed { get; set; }
        public DateTime? TimeOut { get; set; }
        [XmlIgnore]
        public object TheObject { get { return GetMe().ItemObject; } }
    }


    [Serializable]
    public class CachedEntry<tt> : CachedEntryBase, ICacheEntry
    {
        public tt Item { get; set; }
        protected override ICacheEntry GetMe()
        {
            return this;
        }

        public string GetDataString()
        {
            return StartUp.Serializer.Serialize(Item);
        }

        [XmlIgnore]
        public object ItemObject { get { return Item; } }
    }
}
