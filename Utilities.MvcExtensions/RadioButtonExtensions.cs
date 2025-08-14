using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using Utilities.Poco;

namespace Utilities.MvcExtensions
{
    public static class RadioButtonExtensions
    {

        public static HtmlString RadioButtonListFor<TModel, TProperty>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, object htmlAttributes = null
    )
        {
            //ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            //ModelExpressionProvider expressionProvider = new ModelExpressionProvider(htmlHelper.MetadataProvider); 
            //var metadata = expressionProvider.CreateModelExpression(htmlHelper.ViewData, expression);

            var _expressionMetadataProvider = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<ModelExpressionProvider>();
            var mExpress = _expressionMetadataProvider.CreateModelExpression(htmlHelper.ViewData, expression);
            var metaExplorer = mExpress.ModelExplorer; 
            ModelMetadata metadata = metaExplorer.Metadata;



            var sb = new StringBuilder();
            sb.Append("<ol class='RadioList'>");
            foreach (var select in selectList)
            {
                var id = string.Format(
                    "{0}_{1}_{2}",
                    htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix.Replace(".", "_").Replace("[", "_").Replace("]", "_"),
                    metadata.PropertyName,
                    select.Value
                );
                if (htmlAttributes == null)
                {
                    htmlAttributes = new
                    {
                        id
                    };
                }
                var dic = new Dictionary<string, string>();
                if (htmlAttributes != null)
                {
                    dic = htmlAttributes.PropertiesAsDictionary();
                }
                if (!dic.ContainsKey("id"))
                {
                    dic.Add("id", id);
                }
                else
                {
                    dic["id"] = id;
                }
                var dic2 = new Dictionary<string, object>();
                foreach (var k in dic.Keys)
                {
                    dic2.Add(k, dic[k]);
                }
                var radio = htmlHelper.RadioButtonFor(expression, select.Value, dic2).ToHtmlString();
                sb.Append("<li>");
                sb.AppendFormat(
                    "<label for=\"{0}\">{2} {1}</label>",
                    id,
                    "<span class='radioLabelText'>" + WebUtility.HtmlEncode(select.Text) + "</span>",
                    radio
                );
                sb.Append("</li>");
            }
            sb.Append("</ol>");
            return new HtmlString(sb.ToString());
        }
        public static HtmlString RadioButtonListFor<TModel, TProperty>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            //ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            //var metaExplorer = ExpressionMetadataProvider.FromLambdaExpression(expression, htmlHelper.ViewData, htmlHelper.MetadataProvider);
            //ModelMetadata metadata = metaExplorer.Metadata;


            var _expressionMetadataProvider = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<ModelExpressionProvider>();
            var mExpress = _expressionMetadataProvider.CreateModelExpression(htmlHelper.ViewData, expression);
            var metaExplorer = mExpress.ModelExplorer;
            ModelMetadata metadata = metaExplorer.Metadata;

            var sb = new StringBuilder();
            sb.Append("<ol class='RadioList'>");
            foreach (var select in selectList)
            {
                var id = string.Format(
                    "{0}_{1}_{2}",
                    htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix.Replace(".", "_").Replace("[", "_").Replace("]", "_"),
                    metadata.PropertyName,
                    select.Value
                );
                if (htmlAttributes == null)
                {
                    htmlAttributes = new Dictionary<string, object>();
                }
                var dic = htmlAttributes;

                if (!dic.ContainsKey("id"))
                {
                    dic.Add("id", id);
                }
                else
                {
                    dic["id"] = id;
                }
                var dic2 = new Dictionary<string, object>();
                foreach (var k in dic.Keys)
                {
                    dic2.Add(k, dic[k]);
                }
                var radio = htmlHelper.RadioButtonFor(expression, select.Value, dic2).ToHtmlString();
                sb.Append("<li>");
                sb.AppendFormat(
                    "<label for=\"{0}\">{2} {1}</label>",
                    id,
                    "<span class='radioLabelText'>" + WebUtility.HtmlEncode(select.Text) + "</span>",
                    radio
                );
                sb.Append("</li>");
            }
            sb.Append("</ol>");
            return new HtmlString(sb.ToString());
        }

        public static HtmlString RadioButtonList<TModel>(
        this IHtmlHelper<TModel> htmlHelper,
        string name, List<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            // ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var sb = new StringBuilder();
            sb.Append("<ol class='RadioList'>");
            foreach (var select in selectList)
            {
                var id = string.Format(
                    "{0}_{1}_{2}",
                    htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix.Replace(".", "_").Replace("[", "_").Replace("]", "_"),
                    name,
                    select.Value
                );
                if (htmlAttributes == null)
                {
                    htmlAttributes = new Dictionary<string, object>();
                }
                var dic = htmlAttributes;
                if (!dic.ContainsKey("id"))
                {
                    dic.Add("id", id);
                }
                else
                {
                    dic["id"] = id;
                }
                var dic2 = new Dictionary<string, object>();
                foreach (var k in dic.Keys)
                {
                    dic2.Add(k, dic[k]);
                }
                var radio = htmlHelper.RadioButton(name, select.Value, select.Selected, dic2).ToHtmlString();
                sb.Append("<li>");
                sb.AppendFormat(
                    "<label for=\"{0}\">{2} {1}</label>",
                    id,
                    "<span class='radioLabelText'>" + WebUtility.HtmlEncode(select.Text) + "</span>",
                    radio
                );
                sb.Append("</li>");
            }
            sb.Append("</ol>");
            return new HtmlString(sb.ToString());
        }

        public static HtmlString RadioButtonList<TModel>(
        this IHtmlHelper<TModel> htmlHelper,
        string name, List<SelectListItem> selectList, object htmlAttributes = null
    )
        {
            // ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var sb = new StringBuilder();
            sb.Append("<ol class='RadioList'>");
            foreach (var select in selectList)
            {
                var id = string.Format(
                    "{0}_{1}_{2}",
                    htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix.Replace(".", "_").Replace("[", "_").Replace("]", "_"),
                    name,
                    select.Value
                );
                if (htmlAttributes == null)
                {
                    htmlAttributes = new
                    {
                        id
                    };
                }
                var dic = htmlAttributes.PropertiesAsDictionary();
                if (!dic.ContainsKey("id"))
                {
                    dic.Add("id", id);
                }
                else
                {
                    dic["id"] = id;
                }
                var dic2 = new Dictionary<string, object>();
                foreach (var k in dic.Keys)
                {
                    dic2.Add(k, dic[k]);
                }
                var radio = htmlHelper.RadioButton(name, select.Value, select.Selected, dic2).ToHtmlString();
                sb.Append("<li>");
                sb.AppendFormat(
                    "<label for=\"{0}\">{2} {1}</label>",
                    id,
                    "<span class='radioLabelText'>" + WebUtility.HtmlEncode(select.Text) + "</span>",
                    radio
                );
                sb.Append("</li>");
            }
            sb.Append("</ol>");
            return new HtmlString(sb.ToString());
        }



        public static HtmlString RadioYesNoForCheck<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            return htmlHelper.RadioYesNoBooleanFor(expression);

        }

        public static HtmlString RadioYesNoBooleanFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            object htmlAttributes = null)
        {
            //var metaExplorer = ExpressionMetadataProvider.FromLambdaExpression(expression, htmlHelper.ViewData, htmlHelper.MetadataProvider);
            //ModelMetadata metadata = metaExplorer.Metadata;
            if (htmlAttributes == null)
            {
                htmlAttributes = new
                {
                    @class = ""
                };
            }
            //var dic = htmlAttributes.PropertiesAsDictionary();
            //var dic2 = new Dictionary<string, object>();
            //foreach (var k in dic.Keys)
            //{
            //    dic2.Add(k, dic[k]);
            //}

            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem()
            {
                Text = "No",
                Value = "False"
            });
            selectList.Add(new SelectListItem()
            {
                Text = "Yes",
                Value = "True"
            });
            var dropdown = htmlHelper.RadioButtonListFor(expression, selectList, htmlAttributes).ToHtmlString();


            return dropdown;
        }

        public static HtmlString RadioYesNoBoolean<TModel>(this IHtmlHelper<TModel> htmlHelper,
            string name,
            bool? value,
            object htmlAttributes = null)
        {
            // ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            if (htmlAttributes == null)
            {
                htmlAttributes = new
                {
                    @class = ""
                };
            }
            var dic = htmlAttributes.PropertiesAsDictionary();
            var dic2 = new Dictionary<string, object>();
            foreach (var k in dic.Keys)
            {
                dic2.Add(k, dic[k]);
            }

            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem()
            {
                Text = "No",
                Value = "False",
                Selected = value == false
            });
            selectList.Add(new SelectListItem()
            {
                Text = "Yes",
                Value = "True",
                Selected = value == true
            });
            return htmlHelper.RadioButtonList(name, selectList, dic2);
        }
    }
}
