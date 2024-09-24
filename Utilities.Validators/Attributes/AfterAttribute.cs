using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Poco;

namespace Utilities.Validators.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AfterAttribute : ValidationAttribute//, IClientValidatable
    {
        private string _beforeField { get; set; }


        public string getBeforeDateField()
        {
            return _beforeField;
        }


        public AfterAttribute(string BeforeField, string errorMessage)
            : base(errorMessage)
        {
            _beforeField = BeforeField;
            ErrorMessage = errorMessage;
        }

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage;
        }



        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dtStr = validationContext.ObjectInstance.GetValue(_beforeField)?.ToString();
            if (string.IsNullOrWhiteSpace(dtStr))
            {
                return null;
            }
            if (value == null)
            {
                return null;
            }


            var tp = value.GetType();

            var dt = tp.FromString(dtStr);

            if (value.LessThan(dt))
            {
                var message = FormatErrorMessage("");
                return new ValidationResult(message);
            }
            return null;
        }

    }
}
