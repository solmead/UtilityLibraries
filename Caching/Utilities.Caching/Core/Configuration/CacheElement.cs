//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Linq;
//using System.Text;

//namespace Utilities.Caching.Core.Configuration
//{
//    public class CacheElement : ConfigurationElement
//    {
//        [ConfigurationProperty("area", IsKey = true, IsRequired = true)]
//        public CacheArea Area
//        {
//            get { return (CacheArea)this["area"]; }
//            set { this["area"] = value; }
//        }

//        [ConfigurationProperty("class", IsRequired = true, DefaultValue = "")]
//        public string Class
//        {
//            get { return (string)this["class"]; }
//            set { this["class"] = value; }
//        }
//    }
//}
