using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Sql
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class TableValueParameterAttribute : Attribute
    {
        public string TypeName { get; set; }
        public TableValueParameterAttribute(string typeName)
        {
            this.TypeName = typeName;
        }
    }
}
