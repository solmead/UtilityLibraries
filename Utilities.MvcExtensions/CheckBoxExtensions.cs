using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Utilities.MvcExtensions
{
    public static class CheckBoxExtensions
    {

        public static HtmlString CheckBoxList<T>(this IHtmlHelper helper,
                                               string name,
                                               IEnumerable<T> items,
                                               string textField,
                                               string valueField,
                                               IEnumerable<T> selectedItems = null)
        {
            Type itemstype = typeof(T);
            PropertyInfo textfieldInfo = itemstype.GetProperty(textField, typeof(string));
            PropertyInfo valuefieldInfo = itemstype.GetProperty(valueField);

            TagBuilder tag;
            StringBuilder checklist = new StringBuilder();
            foreach (var item in items)
            {
                tag = new TagBuilder("input");
                tag.Attributes["type"] = "checkbox";
                tag.Attributes["value"] = valuefieldInfo.GetValue(item, null).ToString();
                tag.Attributes["name"] = name;
                if (selectedItems != null && selectedItems.Contains(item))
                {
                    tag.Attributes["checked"] = "checked";
                }
                tag.InnerHtml = textfieldInfo.GetValue(item, null).ToString();
                checklist.Append(tag.ToString());
                checklist.Append("<br />");
            }
            return HtmlString.Create(checklist.ToString());
        }


        public static HtmlString CheckBoxList(this IHtmlHelper htmlHelper,
                                               string name,
                                               IEnumerable<SelectListItem> items,
                                               object htmlAttributes)
        {


            if (htmlAttributes == null)
            {
                htmlAttributes = new
                {
                    @class = name
                };
            }
            var dic = htmlAttributes.PropertiesAsDictionary();
            if (!dic.ContainsKey("class"))
            {
                dic.Add("class", name);
            }

            dic["class"] = dic["class"] + " CheckBoxList";

            var ol = new TagBuilder("ol");
            foreach (var k in dic.Keys)
            {
                ol.Attributes.Add(new KeyValuePair<string, string>(k, dic[k]));
            }


            var sb = new StringBuilder();
            foreach (var item in items)
            {
                var id = name;
                var baseId = name + "_" + item.Value;
                if (!string.IsNullOrWhiteSpace(htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix))
                {
                    id = string.Format(
                        "{0}_{1}",
                        htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix,
                        id
                        );
                    baseId = string.Format(
                        "{0}_{1}",
                        htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix,
                        baseId
                        );
                }

                var radio = htmlHelper.CheckBox(id, item.Selected, new { id = baseId, value = item.Value }).ToHtmlString();
                sb.Append("<li>");
                sb.AppendFormat(
                    "<label for=\"{0}\">{2} {1}</label>",
                    baseId,
                    HttpUtility.HtmlEncode(item.Text),
                    radio
                );
                sb.Append("</li>");
            }
            ol.InnerHtml = sb.ToString();

            return HtmlString.Create(ol.ToString());

            //return HtmlString.Create(checklist.ToString());
        }



        public static HtmlString CheckBoxListFor<TModel, TProperty>(
                                 this IHtmlHelper<TModel> htmlHelper,
                                    Expression<Func<TModel, TProperty>> expression,
                                    IEnumerable<SelectListItem> items,
                                    object htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var name = ExpressionHelper.GetExpressionText(expression);
            //var name = metadata.PropertyName;
            //metadata.DisplayName

            if (htmlAttributes == null)
            {
                htmlAttributes = new
                {
                    @class = name
                };
            }
            var dic = htmlAttributes.PropertiesAsDictionary();
            if (!dic.ContainsKey("class"))
            {
                dic.Add("class", name);
            }

            dic["class"] = dic["class"] + " CheckBoxList";

            var ol = new TagBuilder("ol");
            foreach (var k in dic.Keys)
            {
                ol.Attributes.Add(new KeyValuePair<string, string>(k, dic[k]));
            }


            var sb = new StringBuilder();
            foreach (var item in items)
            {
                var id = name;
                var baseId = name + "_" + item.Value;

                if (!string.IsNullOrWhiteSpace(htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix))
                {
                    id = string.Format(
                        "{0}_{1}",
                        htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix,
                        id
                        );
                    baseId = string.Format(
                        "{0}_{1}",
                        htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix,
                        baseId
                        );
                }
                var cbox = htmlHelper.CheckBox(baseId, item.Selected, new { name = id, value = item.Value });
                var check = cbox.ToHtmlString();
                check = check.Replace("name=\"" + baseId + "\"", "name=\"" + id + "\"");

                string pureCheckBox = check.Substring(0, check.IndexOf("<input", 1));
                sb.Append("<li>");
                sb.AppendFormat(
                    "<label for=\"{0}\">{2} {1}</label>",
                    baseId,
                    HttpUtility.HtmlEncode(item.Text),
                    pureCheckBox
                );
                sb.Append("</li>");
            }
            ol.InnerHtml = sb.ToString();

            return HtmlString.Create(ol.ToString());

            //return HtmlString.Create(checklist.ToString());
        }



    }
}
