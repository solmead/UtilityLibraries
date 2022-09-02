using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Utilities.Sql
{
    public class Parameter : DbParameter
    {

        public Parameter(string name, object value)
        {
            ParameterName = name;
            Value = value;
        }

        public override void ResetDbType()
        {
            //throw new NotImplementedException();
           
        }
        public override DbType DbType { get; set; }
        public override ParameterDirection Direction { get; set; }
        public override bool IsNullable { get; set; }
        public override string ParameterName { get; set; }
        public override string SourceColumn { get; set; }
        public override object Value { get; set; }
        public override bool SourceColumnNullMapping { get; set; }
        public override int Size { get; set; }

        

        public SqlDbType? SqlDbTypeEx { get; set; } = null;
        public string TypeNameEx { get; set; } = null;
    }
}
