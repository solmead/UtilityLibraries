using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.EnumExtensions
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class EnumDataAttribute : Attribute
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class EnumDataAttribute<TT> : Attribute
    {
        public string Name { get; set; }
        public TT Value { get; set; }
    }



}
