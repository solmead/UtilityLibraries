using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Utilities.EnumExtensions
{
    public static class Extensions
    {
        public static T ToEnum<T>(this string s) where T : struct, System.Enum
        {
            return Enum.TryParse(s, out T newValue) ? newValue : default(T);
        }
        public static T? ToEnumNullable<T>(this string s) where T : struct, System.Enum
        {
            return Enum.TryParse(s, out T newValue) ? newValue : default(T?);
        }
        public static bool? DataAsBool<TEnum>(this TEnum value, string name) where TEnum : System.Enum
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            EnumDataAttribute<bool>[] attributes2 = (EnumDataAttribute<bool>[])fi?.GetCustomAttributes(typeof(EnumDataAttribute<bool>), false);
            if (attributes2 != null)
            {
                var att = attributes2.Where((a) => a.Name == name).FirstOrDefault();
                if (att != null)
                {
                    return att?.Value ?? false;
                }
            }

            EnumDataAttribute[] attributes = (EnumDataAttribute[])fi?.GetCustomAttributes(typeof(EnumDataAttribute), false);

            if (attributes != null)
            {
                var att = attributes.Where((a) => a.Name == name).FirstOrDefault();
                if (att != null)
                {
                    return (bool.TryParse(att?.Value, out bool t) ? t : (bool?)null);
                }
            }


            return null;
        }
        //public static bool? GetEnumDataAsBool<TEnum>(TEnum value, string name) where TEnum : System.Enum
        //{
        //    FieldInfo fi = value.GetType().GetField(value.ToString());

        //    EnumDataAttribute[] attributes = (EnumDataAttribute[])fi?.GetCustomAttributes(typeof(EnumDataAttribute), false);

        //    if (attributes != null)
        //    {
        //        var att = attributes.Where((a) => a.Name == name).FirstOrDefault();
        //        return (bool.TryParse(att?.Value, out bool t) ? t : (bool?)null);
        //    }

        //    EnumDataAttribute<bool>[] attributes2 = (EnumDataAttribute<bool>[])fi?.GetCustomAttributes(typeof(EnumDataAttribute<bool>), false);
        //    if (attributes2 != null)
        //    {
        //        var att = attributes2.Where((a) => a.Name == name).FirstOrDefault();
        //        return att?.Value ?? false;
        //    }

        //    return null;
        //}
        public static string Data<TEnum>(this TEnum value, string name) where TEnum : System.Enum
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            EnumDataAttribute<string>[] attributes2 = (EnumDataAttribute<string>[])fi?.GetCustomAttributes(typeof(EnumDataAttribute<string>), false);
            if (attributes2 != null)
            {
                var att = attributes2.Where((a) => a.Name == name).FirstOrDefault();
                if (att != null)
                {
                    return att?.Value;
                }
            }

            EnumDataAttribute[] attributes = (EnumDataAttribute[])fi?.GetCustomAttributes(typeof(EnumDataAttribute), false);

            if (attributes != null)
            {
                var att = attributes.Where((a) => a.Name == name).FirstOrDefault();
                if (att != null)
                {
                    return att?.Value;
                }
            }


            return null;
        }
        public static ToEnum DataAsEnum<TEnum, ToEnum>(this TEnum value, string name) 
            where TEnum : System.Enum
            where ToEnum : struct, System.Enum
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            EnumDataAttribute<ToEnum>[] attributes2 = (EnumDataAttribute<ToEnum>[])fi?.GetCustomAttributes(typeof(EnumDataAttribute<ToEnum>), false);
            if (attributes2 != null)
            {
                var att = attributes2.Where((a) => a.Name == name).FirstOrDefault();
                if (att != null)
                {
                    return att.Value;
                }
            }

            EnumDataAttribute[] attributes = (EnumDataAttribute[])fi?.GetCustomAttributes(typeof(EnumDataAttribute), false);

            if (attributes != null)
            {
                var att = attributes.Where((a) => a.Name == name).FirstOrDefault();
                if (att != null)
                {
                    return att.Value?.ToEnum<ToEnum>() ?? default(ToEnum);
                }
            }



            return default(ToEnum);
        }
        //public static ToEnum GetEnumData<TEnum, ToEnum>(TEnum value, string name) 
        //    where TEnum : System.Enum
        //    where ToEnum : struct, System.Enum
        //{
        //    FieldInfo fi = value.GetType().GetField(value.ToString());

        //    EnumDataAttribute[] attributes = (EnumDataAttribute[])fi?.GetCustomAttributes(typeof(EnumDataAttribute), false);

        //    if (attributes != null)
        //    {
        //        var att = attributes.Where((a) => a.Name == name).FirstOrDefault();
        //        return att?.Value?.ToEnum<ToEnum>() ?? default(ToEnum);
        //    }

        //    EnumDataAttribute<ToEnum>[] attributes2 = (EnumDataAttribute<ToEnum>[])fi?.GetCustomAttributes(typeof(EnumDataAttribute<ToEnum>), false);
        //    if (attributes2 != null)
        //    {
        //        var att = attributes2.Where((a) => a.Name == name).FirstOrDefault();
        //        if (att != null)
        //        {
        //            return att.Value;
        //        }
        //    }

        //    return default(ToEnum);
        //}


        public static string GetEnumDescription<TEnum>(TEnum value) where TEnum : System.Enum
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi?.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if ((attributes != null) && (attributes.Length > 0))
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static string Description<TEnum>(this TEnum value) where TEnum : System.Enum
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi?.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if ((attributes != null) && (attributes.Length > 0))
                return attributes[0].Description;
            else
                return value.ToString();
        }
        public static int Order<TEnum>(this TEnum value) where TEnum : System.Enum
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            EnumOrderAttribute[] attributes = (EnumOrderAttribute[])fi?.GetCustomAttributes(typeof(EnumOrderAttribute), false);

            if ((attributes != null) && (attributes.Length > 0))
                return attributes[0].Order;
            else
                return Convert.ToInt32(value);
        }
        public static int GetEnumOrder<TEnum>(TEnum value) where TEnum : System.Enum
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            EnumOrderAttribute[] attributes = (EnumOrderAttribute[])fi?.GetCustomAttributes(typeof(EnumOrderAttribute), false);

            if ((attributes != null) && (attributes.Length > 0))
                return attributes[0].Order;
            else
                return Convert.ToInt32(value);
        }
        public static IEnumerable<TEnum> GetWithOrder<TEnum>(this TEnum enumVal) where TEnum : System.Enum
        {
            return enumVal.GetType().GetWithOrder<TEnum>();
        }

        public static IEnumerable<TEnum> GetWithOrder<TEnum>(this Type type) where TEnum : System.Enum
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
                                        order = fieldInfo.attribute?.Order ?? 0
                                    })
                                   .OrderBy(field => field.order)
                                   .Select(field => field.name);


            return (from s in lst select (TEnum)Enum.Parse(type, s)).ToList();

        }

    }
}