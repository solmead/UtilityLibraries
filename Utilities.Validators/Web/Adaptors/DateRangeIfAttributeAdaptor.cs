using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Validators.Attributes;

namespace Utilities.Validators.Web.Adaptors
{
    public class DateRangeIfAttributeAdaptor : AttributeAdapterBase<DateRangeIfAttribute>
    {
        private readonly string _max;
        private readonly string _min;
        public DateRangeIfAttributeAdaptor(DateRangeIfAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {

            _max = Convert.ToString(Attribute.Maximum, CultureInfo.InvariantCulture)!;
            _min = Convert.ToString(Attribute.Minimum, CultureInfo.InvariantCulture)!;
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-range", GetErrorMessage(context));
            MergeAttribute(context.Attributes, "data-val-range-max", _max);
            MergeAttribute(context.Attributes, "data-val-range-min", _min);
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            return GetErrorMessage(validationContext.ModelMetadata, validationContext.ModelMetadata.GetDisplayName());
        }
    }
}
