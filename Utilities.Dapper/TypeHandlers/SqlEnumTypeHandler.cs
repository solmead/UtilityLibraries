//using Dapper;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Utilities.Dapper.TypeHandlers
//{
//    public class SqlEnumTypeHandler : SqlMapper.TypeHandler<Enum>
//    {
//        public override Enum Parse(object value) => Enum.Parse(, value);// .FromDateTime((DateTime)value);

//        public override void SetValue(IDbDataParameter parameter, Enum value)
//        {
//            parameter.DbType = DbType.String;
//            parameter.Value = value?.ToString();
//        }
//    }
//}
