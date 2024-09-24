using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Utilities.Dapper.TypeHandlers
{
    class CustomTypeMap : SqlMapper.ITypeMap
    {
        private readonly Dictionary<string, SqlMapper.IMemberMap> members
            = new Dictionary<string, SqlMapper.IMemberMap>(StringComparer.OrdinalIgnoreCase);
        public Type Type { get { return type; } }
        private readonly Type type;
        private readonly SqlMapper.ITypeMap tail;
        public void Map(string columnName, string memberName)
        {
            members[columnName] = new MemberMap(type.GetMember(memberName).Single(), columnName);
        }
        public CustomTypeMap(Type type, SqlMapper.ITypeMap tail)
        {
            this.type = type;
            this.tail = tail;
        }
        public ConstructorInfo FindConstructor(string[] names, Type[] types)
        {
            return tail.FindConstructor(names, types);
        }

        public SqlMapper.IMemberMap GetConstructorParameter(
            ConstructorInfo constructor, string columnName)
        {
            return tail.GetConstructorParameter(constructor, columnName);
        }

        public SqlMapper.IMemberMap GetMember(string columnName)
        {
            SqlMapper.IMemberMap map;
            if (!members.TryGetValue(columnName, out map))
            { // you might want to return null if you prefer not to fallback to the
              // default implementation
                map = tail.GetMember(columnName);
            }
            return map;
        }

        public ConstructorInfo FindExplicitConstructor()
        {
            return null;
            //throw new NotImplementedException();
        }
    }
    class MemberMap : SqlMapper.IMemberMap
    {
        private readonly MemberInfo member;
        private readonly string columnName;
        public MemberMap(MemberInfo member, string columnName)
        {
            this.member = member;
            this.columnName = columnName;
        }
        public string ColumnName { get { return columnName; } }
        public FieldInfo Field { get { return member as FieldInfo; } }
        public Type MemberType
        {
            get
            {
                switch (member.MemberType)
                {
                    case MemberTypes.Field: return ((FieldInfo)member).FieldType;
                    case MemberTypes.Property: return ((PropertyInfo)member).PropertyType;
                    default: throw new NotSupportedException();
                }
            }
        }
        public ParameterInfo Parameter { get { return null; } }
        public PropertyInfo Property { get { return member as PropertyInfo; } }
    }
}

