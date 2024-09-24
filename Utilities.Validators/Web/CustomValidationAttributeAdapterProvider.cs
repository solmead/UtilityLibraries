using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Validators.Attributes;
using Utilities.Validators.Web.Adaptors;

namespace Utilities.Validators.Web
{
    public class CustomValidationAttributeAdapterProvider
        : ValidationAttributeAdapterProvider, IValidationAttributeAdapterProvider
    {
        public CustomValidationAttributeAdapterProvider()
        {
        }

        IAttributeAdapter IValidationAttributeAdapterProvider.GetAttributeAdapter(
            ValidationAttribute attribute,
            IStringLocalizer stringLocalizer)
        {
            var adapter = base.GetAttributeAdapter(attribute, stringLocalizer);

            var type = attribute.GetType();
            if (typeof(AfterAttribute).IsAssignableFrom(type))
            {
                return new AfterAttributeAdaptor((AfterAttribute)attribute, stringLocalizer);
            }
            if (typeof(BeforeAttribute).IsAssignableFrom(type))
            {
                return new BeforeAttributeAdaptor((BeforeAttribute)attribute, stringLocalizer);
            }
            if (typeof(NotBothAttribute).IsAssignableFrom(type))
            {
                return new NotBothAttributeAdaptor((NotBothAttribute)attribute, stringLocalizer);
            }



            return adapter;
        }
    }
}
