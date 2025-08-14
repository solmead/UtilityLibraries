using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Utilities.MvcExtensions
{
    public static class EnumExtensions
    {

        private static readonly SelectListItem[] SingleEmptyItem = new[] { new SelectListItem { Text = "", Value = "" } };

        public static string GetEnumDescription<TModel, TEnum>(this IHtmlHelper<TModel> htmlHelper, TEnum value) where TEnum : struct, IConvertible
        {
            return Utilities.EnumExtensions.Extensions.GetEnumDescription(value);
        }
        public static HtmlString EnumDropDownListForEx<TModel, TEnum>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression) where TEnum : struct, IConvertible
        {
            return htmlHelper.EnumDropDownListForEx(expression, null);
        }

        public static HtmlString EnumDropDownListForEx<TModel, TEnum>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, object htmlAttributes) where TEnum : struct, IConvertible
        {
            //var metaExplorer = ExpressionMetadataProvider.FromLambdaExpression(expression, htmlHelper.ViewData, htmlHelper.MetadataProvider);
            //ModelMetadata metadata = metaExplorer.Metadata;

            var _expressionMetadataProvider = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<ModelExpressionProvider>();
            var mExpress = _expressionMetadataProvider.CreateModelExpression(htmlHelper.ViewData, expression);
            var metaExplorer = mExpress.ModelExplorer;
            ModelMetadata metadata = metaExplorer.Metadata;



            Type enumType = Extensions.GetNonNullableModelType(metadata);
            IEnumerable<TEnum> values = Enum.GetValues(enumType).Cast<TEnum>();

            IEnumerable<SelectListItem> items = from value in values
                                                select new SelectListItem
                                                {
                                                    Text = Utilities.EnumExtensions.Extensions.GetEnumDescription(value),
                                                    Value = value.ToString(),
                                                    Selected = value.Equals(metaExplorer.Model)
                                                };

            // If the enum is nullable, add an 'empty' item to the collection
            if (metadata.IsNullableValueType)
                items = SingleEmptyItem.Concat(items);

            return htmlHelper.DropDownListFor(expression, items, htmlAttributes).ToHtmlString();
        }

        public static HtmlString EnumRadioButtonListFor<TModel, TEnum>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TEnum>> expression, object htmlAttributes
    ) where TEnum : struct, IConvertible
        {
            //var metaExplorer = ExpressionMetadataProvider.FromLambdaExpression(expression, htmlHelper.ViewData, htmlHelper.MetadataProvider);
            //ModelMetadata metadata = metaExplorer.Metadata;

            var _expressionMetadataProvider = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<ModelExpressionProvider>();
            var mExpress = _expressionMetadataProvider.CreateModelExpression(htmlHelper.ViewData, expression);
            var metaExplorer = mExpress.ModelExplorer;
            ModelMetadata metadata = metaExplorer.Metadata;



            Type enumType = Extensions.GetNonNullableModelType(metadata);
            IEnumerable<TEnum> values = Enum.GetValues(enumType).Cast<TEnum>();

            IEnumerable<SelectListItem> items = from value in values
                                                select new SelectListItem
                                                {
                                                    Text = Utilities.EnumExtensions.Extensions.GetEnumDescription(value),
                                                    Value = value.ToString(),
                                                    Selected = value.Equals(metaExplorer.Model)
                                                };

            // If the enum is nullable, add an 'empty' item to the collection
            //if (metadata.IsNullableValueType)
            //    items = SingleEmptyItem.Concat(items);

            return htmlHelper.RadioButtonListFor(expression, items, htmlAttributes);
        }
        public static HtmlString EnumRadioButtonList<TModel, TEnum>(
        this IHtmlHelper<TModel> htmlHelper,
        string name,
        TEnum? selectedValue,
        object htmlAttributes = null
    ) where TEnum : struct, IConvertible
        {
            IEnumerable<TEnum> values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

            IEnumerable<SelectListItem> items = from value in values
                                                select new SelectListItem
                                                {
                                                    Text = Utilities.EnumExtensions.Extensions.GetEnumDescription(value),
                                                    Value = value.ToString(),
                                                    Selected = value.Equals(selectedValue)
                                                };

            //items = SingleEmptyItem.Concat(items);

            return htmlHelper.RadioButtonList(name, items.ToList(), htmlAttributes);
        }
        public static HtmlString EnumRadioButtonListFor<TModel, TEnum>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TEnum>> expression
    ) where TEnum : struct, IConvertible
        {
            //var metaExplorer  = ExpressionMetadataProvider.FromLambdaExpression(expression, htmlHelper.ViewData, htmlHelper.MetadataProvider);
            //ModelMetadata metadata = metaExplorer.Metadata;


            var _expressionMetadataProvider = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<ModelExpressionProvider>();
            var mExpress = _expressionMetadataProvider.CreateModelExpression(htmlHelper.ViewData, expression);
            var metaExplorer = mExpress.ModelExplorer;
            ModelMetadata metadata = metaExplorer.Metadata;


            Type enumType = Extensions.GetNonNullableModelType(metadata);
            IEnumerable<TEnum> values = Enum.GetValues(enumType).Cast<TEnum>();

            IEnumerable<SelectListItem> items = from value in values
                                                select new SelectListItem
                                                {
                                                    Text = Utilities.EnumExtensions.Extensions.GetEnumDescription(value),
                                                    Value = value.ToString(),
                                                    Selected = value.Equals(metaExplorer.Model)
                                                };

            // If the enum is nullable, add an 'empty' item to the collection
            if (metadata.IsNullableValueType)
                items = SingleEmptyItem.Concat(items);

            return htmlHelper.RadioButtonListFor(expression, items);
        }
    }
}
