//using Microsoft.AspNetCore.Antiforgery;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.ModelBinding;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.AspNetCore.Mvc.Routing;
//using Microsoft.AspNetCore.Mvc.ViewFeatures;
//using Microsoft.Extensions.Options;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Resources;
//using System.Text;
//using System.Text.Encodings.Web;
//using System.Threading.Tasks;
//using System.Collections;
//using System.ComponentModel.DataAnnotations;
//using System.Diagnostics;
//using System.Globalization;
//using System.Reflection;
//using Microsoft.AspNetCore.Html;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc.ViewFeatures;
//using System.Linq.Expressions;
//using System.Net;
//using System.Xml.Serialization;

//namespace Utilities.MvcExtensions.Generators
//{
//    public class ExtensionsHtmlGenerator : DefaultHtmlGenerator
//    {

//        private IModelMetadataProvider tempMetadataProvider = null;

//        public ExtensionsHtmlGenerator(IAntiforgery antiforgery, IOptions<MvcViewOptions> optionsAccessor, IModelMetadataProvider metadataProvider, IUrlHelperFactory urlHelperFactory, HtmlEncoder htmlEncoder, ValidationHtmlAttributeProvider validationAttributeProvider) : base(antiforgery, optionsAccessor, metadataProvider, urlHelperFactory, htmlEncoder, validationAttributeProvider)
//        {
//            tempMetadataProvider = metadataProvider;
//        }


//        public TagBuilder GenerateRadioList(
//        ViewContext viewContext,
//        ModelExplorer modelExplorer,
//        string optionLabel,
//        string expression,
//        IEnumerable<SelectListItem> selectList,
//        object htmlAttributes)
//        {
//            ArgumentNullException.ThrowIfNull(viewContext);

//            var currentValues = GetCurrentValues(viewContext, modelExplorer, expression, false).FirstOrDefault();
//            return GenerateRadioList(
//                viewContext,
//                modelExplorer,
//                optionLabel,
//                expression,
//                selectList,
//                currentValues,
//                htmlAttributes);
//        }

//        private static bool IsFullNameValid(
//            string fullName,
//            IDictionary<string, object> htmlAttributeDictionary,
//            string fallbackAttributeName)
//        {
//            if (string.IsNullOrEmpty(fullName))
//            {
//                // fullName==null is normally an error because name="" is not valid in HTML 5.
//                if (htmlAttributeDictionary == null)
//                {
//                    return false;
//                }

//                // Check if user has provided an explicit name attribute.
//                // Generalized a bit because other attributes e.g. data-valmsg-for refer to element names.
//                htmlAttributeDictionary.TryGetValue(fallbackAttributeName, out var attributeObject);
//                var attributeString = Convert.ToString(attributeObject, CultureInfo.InvariantCulture);
//                if (string.IsNullOrEmpty(attributeString))
//                {
//                    return false;
//                }
//            }

//            return true;
//        }

//        // Only need a dictionary if htmlAttributes is non-null. TagBuilder.MergeAttributes() is fine with null.
//        private static IDictionary<string, object> GetHtmlAttributeDictionaryOrNull(object htmlAttributes)
//        {
//            IDictionary<string, object> htmlAttributeDictionary = null;
//            if (htmlAttributes != null)
//            {
//                htmlAttributeDictionary = htmlAttributes as IDictionary<string, object>;
//                if (htmlAttributeDictionary == null)
//                {
//                    htmlAttributeDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
//                }
//            }

//            return htmlAttributeDictionary;
//        }


//        public virtual TagBuilder GenerateRadioList(
//        ViewContext viewContext,
//        ModelExplorer modelExplorer,
//        string optionLabel,
//        string expression,
//        IEnumerable<SelectListItem> selectList,
//        string currentValue,
//        object htmlAttributes)
//        {
//            ArgumentNullException.ThrowIfNull(viewContext);

//            var fullName = NameAndIdProvider.GetFullHtmlFieldName(viewContext, expression);
//            var htmlAttributeDictionary = GetHtmlAttributeDictionaryOrNull(htmlAttributes);
//            if (!IsFullNameValid(fullName, htmlAttributeDictionary, "Id"))
//            {
//                throw new Exception("Metadata missing for RadioButtonList");
//               // throw new ArgumentException(
//                    //Resources.FormatHtmlGenerator_FieldNameCannotBeNullOrEmpty(
//                    //    typeof(IHtmlHelper).FullName,
//                    //    nameof(IHtmlHelper.Editor),
//                    //    typeof(IHtmlHelper<>).FullName,
//                    //    nameof(IHtmlHelper<object>.EditorFor),
//                    //    "htmlFieldName"),
//                    //nameof(expression));
//            }

//            // If we got a null selectList, try to use ViewData to get the list of items.
//            if (selectList == null)
//            {
//                selectList = GetSelectListItems(viewContext, expression);
//            }

//            // Convert each ListItem to an <option> tag and wrap them with <optgroup> if requested.
//            //var listItemBuilder = GenerateGroupsAndOptions(optionLabel, selectList);
//            var listItemBuilder = GenerateRadioOptions(optionLabel, selectList, currentValue);

//            modelExplorer = modelExplorer ??
//                ExpressionMetadataProvider.FromStringExpression(expression, viewContext.ViewData, tempMetadataProvider);


//            var tagBuilder = new TagBuilder("ol");
//            tagBuilder.AddCssClass("RadioList");
//            tagBuilder.InnerHtml.SetHtmlContent(listItemBuilder);
//            tagBuilder.MergeAttributes(htmlAttributeDictionary);
//            NameAndIdProvider.GenerateId(viewContext, tagBuilder, fullName, IdAttributeDotReplacement);
//            if (!string.IsNullOrEmpty(fullName))
//            {
//                tagBuilder.MergeAttribute("name", fullName, replaceExisting: true);
//            }


//            // If there are any errors for a named field, we add the css attribute.
//            if (viewContext.ViewData.ModelState.TryGetValue(fullName, out var entry))
//            {
//                if (entry.Errors.Count > 0)
//                {
//                    tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
//                }
//            }

//            AddValidationAttributes(viewContext, tagBuilder, modelExplorer, expression);

//            return tagBuilder;
//        }


//        private static IEnumerable<SelectListItem> GetSelectListItems(
//    ViewContext viewContext,
//    string expression)
//        {
//            ArgumentNullException.ThrowIfNull(viewContext);

//            // Method is called only if user did not pass a select list in. They must provide select list items in the
//            // ViewData dictionary and definitely not as the Model. (Even if the Model datatype were correct, a
//            // <select> element generated for a collection of SelectListItems would be useless.)
//            var value = viewContext.ViewData.Eval(expression);

//            // First check whether above evaluation was successful and did not match ViewData.Model.
//            if (value == null || value == viewContext.ViewData.Model)
//            {

//                throw new Exception("MissingSelectData missing for RadioButtonList");
//                //throw new InvalidOperationException(Resources.FormatHtmlHelper_MissingSelectData(
//                //    $"IEnumerable<{nameof(SelectListItem)}>",
//                //    expression));
//            }

//            // Second check the Eval() call returned a collection of SelectListItems.
//            if (!(value is IEnumerable<SelectListItem> selectList))
//            {
//                throw new Exception("WrongSelectDataType missing for RadioButtonList");
//                //throw new InvalidOperationException(Resources.FormatHtmlHelper_WrongSelectDataType(
//                //    expression,
//                //    value.GetType().FullName,
//                //    $"IEnumerable<{nameof(SelectListItem)}>"));
//            }

//            return selectList;
//        }

//        private static IHtmlContent GenerateRadioOptions(
//        ViewContext viewContext,
//        ModelExplorer modelExplorer,
//        string optionLabel,
//        IEnumerable<SelectListItem> selectList,
//        string currentValue)
//        {
//            ModelMetadata metadata = modelExplorer.Metadata;

//            if (!(selectList is IList<SelectListItem> itemsList))
//            {
//                itemsList = selectList.ToList();
//            }

//            var count = itemsList.Count;
//            if (optionLabel != null)
//            {
//                count++;
//            }

//            // Short-circuit work below if there's nothing to add.
//            if (count == 0)
//            {
//                return HtmlString.Empty;
//            }

//            var listItemBuilder = new HtmlContentBuilder(count);


//            var sb = new StringBuilder();
//            foreach (var select in itemsList)
//            {
//                var id = string.Format(
//                    "{0}_{1}_{2}",
//                    htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix.Replace(".", "_").Replace("[", "_").Replace("]", "_"),
//                metadata.PropertyName,
//                    select.Value
//                );
//                if (htmlAttributes == null)
//                {
//                    htmlAttributes = new
//                    {
//                        id
//                    };
//                }
//                var dic = new Dictionary<string, string>();
//                if (htmlAttributes != null)
//                {
//                    dic = htmlAttributes.PropertiesAsDictionary();
//                }
//                if (!dic.ContainsKey("id"))
//                {
//                    dic.Add("id", id);
//                }
//                else
//                {
//                    dic["id"] = id;
//                }
//                var dic2 = new Dictionary<string, object>();
//                foreach (var k in dic.Keys)
//                {
//                    dic2.Add(k, dic[k]);
//                }
//                var radio = htmlHelper.RadioButtonFor(expression, select.Value, dic2).ToHtmlString();
//                sb.Append("<li>");
//                sb.AppendFormat(
//                    "<label for=\"{0}\">{2} {1}</label>",
//                    id,
//                    "<span class='radioLabelText'>" + WebUtility.HtmlEncode(select.Text) + "</span>",
//                    radio
//                );
//                sb.Append("</li>");
//            }



//            //// Make optionLabel the first item that gets rendered.
//            //if (optionLabel != null)
//            //{
//            //    listItemBuilder.AppendLine(GenerateOption(
//            //        new SelectListItem()
//            //        {
//            //            Text = optionLabel,
//            //            Value = string.Empty,
//            //            Selected = false,
//            //        },
//            //        currentValues: null));
//            //}

//            //// Group items in the SelectList if requested.
//            //// The worst case complexity of this algorithm is O(number of groups*n).
//            //// If there aren't any groups, it is O(n) where n is number of items in the list.
//            //var optionGenerated = new bool[itemsList.Count];
//            //for (var i = 0; i < itemsList.Count; i++)
//            //{
//            //    if (!optionGenerated[i])
//            //    {
//            //        var item = itemsList[i];
//            //        var optGroup = item.Group;
//            //        if (optGroup != null)
//            //        {
//            //            var groupBuilder = new TagBuilder("optgroup");
//            //            if (optGroup.Name != null)
//            //            {
//            //                groupBuilder.MergeAttribute("label", optGroup.Name);
//            //            }

//            //            if (optGroup.Disabled)
//            //            {
//            //                groupBuilder.MergeAttribute("disabled", "disabled");
//            //            }

//            //            groupBuilder.InnerHtml.AppendLine();

//            //            for (var j = i; j < itemsList.Count; j++)
//            //            {
//            //                var groupItem = itemsList[j];

//            //                if (!optionGenerated[j] &&
//            //                    object.ReferenceEquals(optGroup, groupItem.Group))
//            //                {
//            //                    groupBuilder.InnerHtml.AppendLine(GenerateOption(groupItem, currentValues));
//            //                    optionGenerated[j] = true;
//            //                }
//            //            }

//            //            listItemBuilder.AppendLine(groupBuilder);
//            //        }
//            //        else
//            //        {
//            //            listItemBuilder.AppendLine(GenerateOption(item, currentValues));
//            //            optionGenerated[i] = true;
//            //        }
//            //    }
//            //}

//            return listItemBuilder;
//        }
//    }
//}
