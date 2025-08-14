using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Utilities.MvcExtensions
{
    public static class SelectExtensions
    {

        public static HtmlString DropDownYesNoForCheck<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            return htmlHelper.DropDownYesNoBooleanFor(expression);

        }

        public static HtmlString DropDownYesNoBooleanFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            object htmlAttributes = null)
        {

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
            var dropdown = htmlHelper.DropDownListFor(expression, selectList, htmlAttributes).ToHtmlString();


            return dropdown;
        }
        public static HtmlString DropDownYesNoBoolean<TModel>(this IHtmlHelper<TModel> htmlHelper,
            string id,
            bool value,
            object htmlAttributes = null)
        {
            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem()
            {
                Text = "No",
                Value = "False",
                Selected = !value
            });
            selectList.Add(new SelectListItem()
            {
                Text = "Yes",
                Value = "True",
                Selected = value
            });
            var dropdown = htmlHelper.DropDownList(id, selectList, htmlAttributes).ToHtmlString();


            return dropdown;
        }

    }
}
