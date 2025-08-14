using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Utilities.Poco;

namespace Utilities.MvcExtensions
{
    public static class LabelExtensions
    {

        public static HtmlString LabelEx(this IHtmlHelper html, string expression, string labelText, object htmlAttributes)
        {
            if (string.IsNullOrEmpty(labelText))
            {
                return HtmlString.Empty;
            }
            //var sb = new StringBuilder();
            //sb.Append(labelText);
            //sb.Append(":");

            var tag = new TagBuilder("label");
            //if (!string.IsNullOrWhiteSpace(id))
            //{
            //    tag.Attributes.Add("id", id);
            //}
            //else if (generatedId)
            //{
            //    tag.Attributes.Add("id", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName) + "_Label");
            //}
            if (htmlAttributes == null)
            {
                htmlAttributes = new
                {
                    @class = ""
                };
            }
            var dic = htmlAttributes.PropertiesAsDictionary();

            var id = html.GenerateIdFromName(html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expression));

            tag.Attributes.Add("for", id);
            tag.MergeAttributes(dic, true);
            
            tag.InnerHtml.AppendHtml(labelText);

            tag.TagRenderMode = TagRenderMode.Normal;
            return tag.ToHtmlString();
        }
        //public static HtmlString LabelEx(this IHtmlHelper html, string expression)
        //{
        //    ModelExpressionProvider expressionProvider = new ModelExpressionProvider(html.MetadataProvider);
        //    var metadata = expressionProvider.CreateModelExpression(html.ViewData, expression);
        //    //metadata.ModelExplorer.
        //    //var metadata2 = _modelExpressionProvider.CreateModelExpression(ViewData, expression).Metadata;
        //    //var modelExplorer = ExpressionMetadataProvider.FromLambdaExpression(expression, html.ViewData, html.MetadataProvider);

        //    //var m = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IModelExpressionProvider>().CreateModelExpression(html.ViewData, expression).ModelExplorer.Model;

        //    return LabelHelper(html, ModelMetadata.FromStringExpression(expression, html.ViewData), expression);
        //}

        public static HtmlString LabelForEx<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var metadata = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IModelExpressionProvider>().CreateModelExpression(html.ViewData, expression).ModelExplorer.Metadata;

            return LabelHelper(html, metadata, html.GetExpressionText(expression));
        }

        public static HtmlString LabelForEx<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string labelText)
        {
            var metadata = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IModelExpressionProvider>().CreateModelExpression(html.ViewData, expression).ModelExplorer.Metadata;
            return LabelHelper(html, metadata, html.GetExpressionText(expression), labelText);
        }
        public static HtmlString LabelForEx<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            var dic = new Dictionary<string, string>();
            if (htmlAttributes != null)
            {
                htmlAttributes.Keys.ToList().ForEach((key) =>
                {
                    dic.Add(key, htmlAttributes[key].ToString());
                });
            }
            var metadata = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IModelExpressionProvider>().CreateModelExpression(html.ViewData, expression).ModelExplorer.Metadata;
            return LabelHelper(html, metadata, html.GetExpressionText(expression), htmlAttributes: dic);
        }
        public static HtmlString LabelForEx<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string labelText, IDictionary<string, object> htmlAttributes)
        {

            var dic = new Dictionary<string, string>();
            if (htmlAttributes != null)
            {
                htmlAttributes.Keys.ToList().ForEach((key) =>
                {
                    dic.Add(key, htmlAttributes[key].ToString());
                });
            }

            var metadata = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IModelExpressionProvider>().CreateModelExpression(html.ViewData, expression).ModelExplorer.Metadata;
            return LabelHelper(html, metadata, html.GetExpressionText(expression), labelText, dic);
        }
        public static HtmlString LabelForEx<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string labelText, object htmlAttributes)
        {
            if (htmlAttributes == null)
            {
                htmlAttributes = new
                {
                    @class = ""
                };
            }
            var dic = htmlAttributes.PropertiesAsDictionary();
            var metadata = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IModelExpressionProvider>().CreateModelExpression(html.ViewData, expression).ModelExplorer.Metadata;
            return LabelHelper(html, metadata, html.GetExpressionText(expression), labelText, dic);
        }
        public static HtmlString LabelForEx<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            if (htmlAttributes == null)
            {
                htmlAttributes = new
                {
                    @class = ""
                };
            }
            var dic = htmlAttributes.PropertiesAsDictionary();
            var metadata = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IModelExpressionProvider>().CreateModelExpression(html.ViewData, expression).ModelExplorer.Metadata;
            return LabelHelper(html, metadata, html.GetExpressionText(expression), htmlAttributes: dic);
        }

        internal static HtmlString LabelHelper(IHtmlHelper html, ModelMetadata metadata, string htmlFieldName, string labelText = null, IDictionary<string, string> htmlAttributes = null)
        {
            labelText = labelText ?? metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            if (string.IsNullOrEmpty(labelText))
            {
                return HtmlString.Empty;
            }
            var sb = new StringBuilder();
            sb.Append(labelText);
            sb.Append(":");

            var tag = new TagBuilder("label");
            tag.MergeAttributes(htmlAttributes);
            //if (!string.IsNullOrWhiteSpace(id))
            //{
            //    tag.Attributes.Add("id", id);
            //}
            //else if (generatedId)
            //{
            //    tag.Attributes.Add("id", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName) + "_Label");
            //}

            var id = html.GenerateIdFromName(htmlFieldName);
            tag.Attributes.Add("for", id);
            //tag.SetInnerText();

            tag.InnerHtml.AppendHtml(sb.ToString());

            var span = new TagBuilder("span");
            span.AddCssClass("requiredStar");
            span.InnerHtml.AppendHtml("*");
            //span.SetInnerText("*");
            if (metadata.IsRequired)
                tag.InnerHtml.AppendHtml(span.ToString());
            
            return new HtmlString(tag.ToString());
        }


        public static HtmlString HtmlLabelFor<TModel, TProperty>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TProperty>> expression
    )
        {
            var metadata = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IModelExpressionProvider>().CreateModelExpression(htmlHelper.ViewData, expression).ModelExplorer.Metadata;
            //var metadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            var htmlFieldName = htmlHelper.GetExpressionText(expression);
            var labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split(new char[] { '.' }).Last<string>();
            if (string.IsNullOrEmpty(labelText))
            {
                return HtmlString.Empty;
            }
            var label = new TagBuilder("label");
            var id = htmlHelper.GenerateIdFromName(htmlFieldName);
            label.Attributes.Add("for", TagBuilder.CreateSanitizedId(id, ""));
            label.InnerHtml.AppendHtml(labelText);
            return new HtmlString(label.ToString());

        }
    }
}
