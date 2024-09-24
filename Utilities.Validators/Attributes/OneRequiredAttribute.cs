using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Poco;

namespace Utilities.Validators.Attributes
{
    public class OneRequiredAttribute : RequiredAttribute
    {
        private string _otherField = null;
        public OneRequiredAttribute(string OtherField)
        {
            _otherField = OtherField;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var valid = base.IsValid(value, validationContext);
            if (valid == null)
            {
                return valid;
            }

            var obj = validationContext.ObjectInstance;

            var v = obj.GetValue(_otherField);

            if (v == null)
            {
                return valid;
            }

            return ValidationResult.Success;
        }
    }
}
