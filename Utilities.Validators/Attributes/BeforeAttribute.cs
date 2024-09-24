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
    public class BeforeAttribute : ValidationAttribute//, IClientValidatable
    {
        private string _afterDateField { get; set; }

        public string getAfterDateField()
        {
            return _afterDateField;
        }

        public BeforeAttribute(string AfterDateField, string errorMessage)
            : base(errorMessage)
        {
            _afterDateField = AfterDateField;
            ErrorMessage = errorMessage;
        }

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dtStr = validationContext.ObjectInstance.GetValue(_afterDateField)?.ToString();
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


            //DateTime dt;
            //DateTime.TryParse(dtStr, out dt);

            //var dateEntered = (DateTime)value;
            if (dt.LessThan(value))
            {
                var message = FormatErrorMessage("");
                return new ValidationResult(message);
            }
            return null;
        }

        //public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        //{
        //    var rule = new ModelClientBeforeValidationRule(_afterDateField, FormatErrorMessage(metadata.DisplayName));
        //    yield return rule;
        //}
    }

    //public sealed class ModelClientBeforeValidationRule : ModelClientValidationRule
    //{


    //    public ModelClientBeforeValidationRule(string otherproperty, string errorMessage)
    //    {
    //        ErrorMessage = errorMessage;
    //        ValidationType = "before";



    //        ValidationParameters.Add("otherproperty", otherproperty);
    //    }
    //}
}
