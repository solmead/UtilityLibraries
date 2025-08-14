using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.MvcExtensions.TagHelpers
{
    public class LabelExTagHelper : LabelTagHelper
    {

        public bool IgnoreRequired { get; set; } = false;

        public LabelExTagHelper(IHtmlGenerator generator): base(generator)
        { 

        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "label";
            await base.ProcessAsync(context, output);


            var metadata = For.ModelExplorer.Metadata;


            var span = new TagBuilder("span");
            span.AddCssClass("requiredStar");
            span.InnerHtml.SetHtmlContent("*");
            //span.SetInnerText("*");
            if (metadata.IsRequired && !IgnoreRequired)
                output.Content.AppendHtml(span.ToHtmlString());


        }


    }
}
