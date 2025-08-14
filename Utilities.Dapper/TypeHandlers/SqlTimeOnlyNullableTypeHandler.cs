using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Dapper.TypeHandlers
{

    //#if NET_6
    public class SqlTimeOnlyNullableTypeHandler : SqlMapper.TypeHandler<TimeOnly?>
    {
        public override void SetValue(IDbDataParameter parameter, TimeOnly? time)
        {
            parameter.Value = time.ToString();
        }

        public override TimeOnly? Parse(object value)
        {
            return (value==null ? null : TimeOnly.FromTimeSpan((TimeSpan)value));
        }
    }
    //#endif
}
