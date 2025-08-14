using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;

namespace Utilities.MvcExtensions
{
    public class HtmlFieldPrefixScope : IDisposable
    {
        private readonly TemplateInfo templateInfo;
        private readonly string previousHtmlFieldPrefix;

        public HtmlFieldPrefixScope(TemplateInfo templateInfo, string htmlFieldPrefix, bool stackScopes = true)
        {
            this.templateInfo = templateInfo;

            previousHtmlFieldPrefix = templateInfo.HtmlFieldPrefix;
            if (string.IsNullOrWhiteSpace(previousHtmlFieldPrefix) || !stackScopes)
            {
                templateInfo.HtmlFieldPrefix = htmlFieldPrefix;
            }
            else
            {
                templateInfo.HtmlFieldPrefix = previousHtmlFieldPrefix + "." + htmlFieldPrefix;
            }
        }

        public void Dispose()
        {
            templateInfo.HtmlFieldPrefix = previousHtmlFieldPrefix;
        }
    }
}