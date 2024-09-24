using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Validators.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class BirthDateAttribute : ValidationAttribute//, IClientValidatable
    {

        private const string DefaultErrorMessage = "Date selected {0} must be before today";

        public BirthDateAttribute()
            : base(DefaultErrorMessage)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(DefaultErrorMessage, name);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var dateEntered = (DateTime)value;
                if (dateEntered >= DateTime.Today)
                {
                    var message = FormatErrorMessage(dateEntered.ToShortDateString());
                    return new ValidationResult(message);
                }
            }
            return null;
        }

        //public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        //{
        //    var rule = new ModelClientBirthdateValidationRule(FormatErrorMessage(metadata.DisplayName));
        //    yield return rule;
        //}
    }

    //public sealed class ModelClientBirthdateValidationRule : ModelClientValidationRule
    //{

    //    public ModelClientBirthdateValidationRule(string errorMessage)
    //    {
    //        ErrorMessage = errorMessage;
    //        ValidationType = "birthdate";
    //    }
    //}
}
