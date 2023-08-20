﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Dapper.TypeHandlers
{
    #if NET_6
    public class SqlDateOnlyNullableTypeHandler : SqlMapper.TypeHandler<DateOnly?>
    {
        public override void SetValue(IDbDataParameter parameter, DateOnly? time)
        {
            parameter.Value = time.ToString();
        }

        public override DateOnly? Parse(object value)
        {
            if (value == null) return null;

            return DateOnly.FromDateTime((DateTime)value);
        }
    }
    #endif
}