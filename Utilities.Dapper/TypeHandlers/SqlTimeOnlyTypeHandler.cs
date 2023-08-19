using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Dapper.TypeHandlers
{

    #if NET_6
    public class SqlTimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly>
    {
        public override TimeOnly Parse(object value)
        {
            if (value.GetType() == typeof(DateTime))
            {
                return TimeOnly.FromDateTime((DateTime)value);
            }
            else if (value.GetType() == typeof(TimeSpan))
            {
                return TimeOnly.FromTimeSpan((TimeSpan)value);
            }
            return default;
        }

        public override void SetValue(IDbDataParameter parameter, TimeOnly value)
        {
            parameter.DbType = DbType.Time;
            parameter.Value = value;
        }
    }
    #endif
}
