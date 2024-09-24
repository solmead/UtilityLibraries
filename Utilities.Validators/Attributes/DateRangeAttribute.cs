using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Poco;

namespace Utilities.Validators.Attributes
{
    public class DateRangeAttribute : RangeAttribute
    {



        public DateRangeAttribute(int daysBeforeToday, int daysAfterToday) : base(typeof(DateTime), DateTime.Now.AddDays(-daysBeforeToday).ToShortDateString(), DateTime.Now.AddDays(daysAfterToday).ToShortDateString())
        {

        }

        public DateRangeAttribute(string minimum, string maximum) : base(typeof(DateTime), minimum, maximum)
        {

        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var v = (DateTime?)value;
            if (v == null)
            {
                return ValidationResult.Success;
            }
            var valid = base.IsValid(value, validationContext);
            if (valid == null)
            {
                return valid;
            }
            var maxDate = DateTime.TryParse(Maximum.ToString(), out DateTime t) ? t : DateTime.MaxValue;
            var minDate = DateTime.TryParse(Minimum.ToString(), out DateTime t2) ? t2 : DateTime.MinValue;

            if (!(minDate <= v && v <= maxDate))
            {
                string[] memberNames = validationContext.MemberName != null ? new string[] { validationContext.MemberName } : null;
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), memberNames);
            }


            return ValidationResult.Success;
        }

    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
    public class DateRangeIfAttribute : RangeAttribute
    {

        private string _otherField = null;
        public string FieldValue { get; set; } = null;
        public bool AnyValue { get; set; } = false;
        public bool IsNot { get; set; } = false;


        public DateRangeIfAttribute(int daysFromToday, string OtherField, string fieldValue = null, bool anyValue = false) : base(typeof(DateTime), DateTime.Now.ToShortDateString(), DateTime.Now.AddDays(daysFromToday).ToShortDateString())
        {

        }

        public DateRangeIfAttribute(string minimum, string maximum, string OtherField, string fieldValue = null, bool anyValue = false) : base(typeof(DateTime), minimum, maximum)
        {
            _otherField = OtherField;
            FieldValue = fieldValue;
            AnyValue = anyValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var valid = base.IsValid(value, validationContext);
            if (valid == null)
            {
                return valid;
            }

            var obj = validationContext.ObjectInstance;

            var v = obj.GetValue(_otherField)?.ToString();

            var isRequiredPositive = AnyValue && !string.IsNullOrWhiteSpace(v) || !AnyValue && FieldValue?.ToUpper() == v?.ToUpper();
            var isRequiredNegative = AnyValue && string.IsNullOrWhiteSpace(v) || !AnyValue && FieldValue?.ToUpper() != v?.ToUpper();

            if (!IsNot && isRequiredPositive || IsNot && isRequiredNegative)
            {
                return valid;
            }


            return ValidationResult.Success;
        }

    }
}
