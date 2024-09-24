using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Poco;

namespace Utilities.Validators.Attributes
{
    public class NotBothAttribute : ValidationAttribute//, IClientValidatable
    {
        private string _otherField { get; set; }

        public string getOtherField()
        {
            return _otherField;
        }

        public NotBothAttribute(string otherField, string errorMessage)
            : base(errorMessage)
        {
            _otherField = otherField;
            ErrorMessage = errorMessage;
        }

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dtStr = validationContext.ObjectInstance.GetValue(_otherField)?.ToString();
            if (string.IsNullOrWhiteSpace(dtStr))
            {
                return null;
            }
            if (value == null)
            {
                return null;
            }

            if (value != null && !string.IsNullOrWhiteSpace(dtStr))
            {
                var message = FormatErrorMessage("");
                return new ValidationResult(message);
            }
            return null;
        }

    }
}
