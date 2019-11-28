using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.EnumExtensions
{
    public class EnumEntry<tEnum> where tEnum : struct, IConvertible
    {
        public tEnum Value { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public EnumEntry()
        {

        }
        public EnumEntry(tEnum value)
        {
            Value = value;
            var n = Convert.ChangeType(value, value.GetTypeCode());
            if (n != null)
            {
                Number = (int)n;
            }
            Name = value.ToString();
            Description = Extensions.GetEnumDescription<tEnum>(value);

        }
        public static List<EnumEntry<tEnum>> GetList()
        {
            Type enumType = typeof(tEnum);
            var values = enumType.GetWithOrder<tEnum>();
            //IEnumerable<tEnum> values = Enum.GetValues(enumType).Cast<tEnum>();
            return values.Select(i => new EnumEntry<tEnum>(i)).ToList();
        }
    }
}
