using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Utilities.EnumExtensions
{
    public static class Extensions
    {
        public static T ToEnum<T>(this string s) where T : struct, IConvertible
        {
            return Enum.TryParse(s, out T newValue) ? newValue : default(T);
        }
        public static T? ToEnumNullable<T>(this string s) where T : struct, IConvertible
        {
            return Enum.TryParse(s, out T newValue) ? newValue : default(T?);
        }

        public static string GetEnumDescription<TEnum>(TEnum value) where TEnum : struct, IConvertible
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if ((attributes != null) && (attributes.Length > 0))
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static string Description<TEnum>(this TEnum value) where TEnum : struct, IConvertible
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if ((attributes != null) && (attributes.Length > 0))
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static IEnumerable<TEnum> GetWithOrder<TEnum>(this TEnum enumVal) where TEnum : struct, IConvertible
        {
            return enumVal.GetType().GetWithOrder<TEnum>();
        }

        public static IEnumerable<TEnum> GetWithOrder<TEnum>(this Type type) where TEnum : struct, IConvertible
        {
            if (!type.IsEnum)
            {
                throw new ArgumentException("Type must be an enum");
            }
            // caching for result could be useful
            var lst = type.GetFields()
                                   .Where(field => field.IsStatic)
                                   .Select(field => new
                                   {
                                       field,
                                       attribute = field.GetCustomAttribute<EnumOrderAttribute>()
                                   })
                                    .Select(fieldInfo => new
                                    {
                                        name = fieldInfo.field.Name,
                                        order = fieldInfo.attribute != null ? fieldInfo.attribute.Order : 0
                                    })
                                   .OrderBy(field => field.order)
                                   .Select(field => field.name);


            return (from s in lst select (TEnum)Enum.Parse(type, s)).ToList();

        }

    }
}