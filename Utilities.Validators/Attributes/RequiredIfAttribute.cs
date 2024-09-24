using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Poco;

namespace Utilities.Validators.Attributes
{
    public class RequiredIfAttribute : RequiredAttribute
    {
        private string _otherField = null;
        public string FieldValue { get; set; } = null;
        public bool AnyValue { get; set; } = false;
        public bool IsNot { get; set; } = false;
        public RequiredIfAttribute(string OtherField, string fieldValue = null, bool anyValue = false)
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
