using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Poco;

namespace Utilities.Validators.Attributes
{
    public class RequiredIfParentRequiredAttribute : RequiredAttribute
    {


        public RequiredIfParentRequiredAttribute() { }


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var valid = base.IsValid(value, validationContext);
            if (valid == null)
            {
                return valid;
            }

            var obj = validationContext.ObjectInstance;

            string v = null;

            if (v == null)
            {
                return valid;
            }

            return ValidationResult.Success;
        }
    }

}
