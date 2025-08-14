using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using Utilities.MvcExtensions;
using Utilities.Poco;

namespace Utilities.MvcExtensions
{
    public static class Extensions
    {
        public static string GetExpressionText<TModel, TResult>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TResult>> expression)
        {
            var expressionProvider = htmlHelper.ViewContext.HttpContext.RequestServices
                .GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;

            return expressionProvider.GetExpressionText(expression);
        }

        public static HtmlString ToHtmlString(this IHtmlContent htmlContent)
        {
            if (htmlContent is HtmlString htmlString)
            {
                return htmlString;
            }

            using (var writer = new StringWriter())
            {
                htmlContent.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
                return new HtmlString( writer.ToString());
            }
        }

        public static Dictionary<string, string> ToDictionary(this FormCollection collection)
        {
            var Dic = new Dictionary<string, string>();
            foreach (string C in collection.Keys)
            {
                Dic.Add(C, collection[C]);
            }
            return Dic;
        }

        public static HtmlString AntiForgeryTokenValue(this IHtmlHelper HtmlHelper)
        {
            var field = HtmlHelper.AntiForgeryToken().ToHtmlString();
            var beginIndex = field.Value.IndexOf("value=\"") + 7;
            var endIndex = field.Value.IndexOf("\"", beginIndex);
            return new HtmlString(field.Value.Substring(beginIndex, endIndex - beginIndex));
        }
        
        
        public static IDisposable BeginHtmlFieldPrefixScope(this IHtmlHelper html, string htmlFieldPrefix, bool stackScopes = true)
        {
            return new HtmlFieldPrefixScope(html.ViewData.TemplateInfo, htmlFieldPrefix, stackScopes);
        }

        internal static Type GetNonNullableModelType(ModelMetadata modelMetadata)
        {
            Type realModelType = modelMetadata.ModelType;

            Type underlyingType = Nullable.GetUnderlyingType(realModelType);
            if (underlyingType != null)
            {
                realModelType = underlyingType;
            }
            return realModelType;
        }


        //public static HtmlString DisplayColumnNameFor<TModel, TClass, TProperty>(this IHtmlHelper<TModel> helper, IEnumerable<TClass> model, Expression<Func<TClass, TProperty>> expression)
        //{
        //    var name = ExpressionHelper.GetExpressionText(expression);
        //    name = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
        //    var metadata = ModelMetadataProviders.Current.GetMetadataForProperty(
        //        () => Activator.CreateInstance<TClass>(), typeof(TClass), name);

        //    return new HtmlString(metadata.DisplayName);
        //}

    }
}