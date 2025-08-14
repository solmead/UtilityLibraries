using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Utilities.MvcExtensions.Generators;

namespace Utilities.MvcExtensions.TagHelpers
{
    //[HtmlTargetElement("radioButtonList", Attributes = ForAttributeName)]
    //[HtmlTargetElement("radioButtonList", Attributes = ItemsAttributeName)]
    //public class RadioButtonListHelper : TagHelper
    //{
    //    private const string ForAttributeName = "asp-for";
    //    private const string ItemsAttributeName = "asp-items";
    //    private string _currentValue;

    //    private ExtensionsHtmlGenerator exGenerator = null;

    //    /// <summary>
    //    /// Creates a new <see cref="SelectTagHelper"/>.
    //    /// </summary>
    //    /// <param name="generator">The <see cref="IHtmlGenerator"/>.</param>
    //    public RadioButtonListHelper(IHtmlGenerator generator, IAntiforgery antiforgery, IOptions<MvcViewOptions> optionsAccessor, IModelMetadataProvider metadataProvider, IUrlHelperFactory urlHelperFactory, HtmlEncoder htmlEncoder, ValidationHtmlAttributeProvider validationAttributeProvider)
    //    {
    //        Generator = generator;
    //        exGenerator = new ExtensionsHtmlGenerator(antiforgery, optionsAccessor, metadataProvider, urlHelperFactory, htmlEncoder, validationAttributeProvider);
    //    }

    //    /// <inheritdoc />
    //    public override int Order => -1000;

    //    /// <summary>
    //    /// Gets the <see cref="IHtmlGenerator"/> used to generate the <see cref="SelectTagHelper"/>'s output.
    //    /// </summary>
    //    protected IHtmlGenerator Generator { get; }

    //    /// <summary>
    //    /// Gets the <see cref="Rendering.ViewContext"/> of the executing view.
    //    /// </summary>
    //    [HtmlAttributeNotBound]
    //    [ViewContext]
    //    public ViewContext ViewContext { get; set; }

    //    /// <summary>
    //    /// An expression to be evaluated against the current model.
    //    /// </summary>
    //    [HtmlAttributeName(ForAttributeName)]
    //    public ModelExpression For { get; set; }

    //    /// <summary>
    //    /// A collection of <see cref="SelectListItem"/> objects used to populate the &lt;select&gt; element with
    //    /// &lt;optgroup&gt; and &lt;option&gt; elements.
    //    /// </summary>
    //    [HtmlAttributeName(ItemsAttributeName)]
    //    public IEnumerable<SelectListItem> Items { get; set; }

    //    /// <summary>
    //    /// The name of the &lt;input&gt; element.
    //    /// </summary>
    //    /// <remarks>
    //    /// Passed through to the generated HTML in all cases. Also used to determine whether <see cref="For"/> is
    //    /// valid with an empty <see cref="ModelExpression.Name"/>.
    //    /// </remarks>
    //    public string Name { get; set; }

    //    /// <inheritdoc />
    //    public override void Init(TagHelperContext context)
    //    {
    //        ArgumentNullException.ThrowIfNull(context);

    //        if (For == null)
    //        {
    //            // Informs contained elements that they're running within a targeted <select/> element.
    //            context.Items[typeof(SelectTagHelper)] = null;
    //            return;
    //        }

    //        // Note null or empty For.Name is allowed because TemplateInfo.HtmlFieldPrefix may be sufficient.
    //        // IHtmlGenerator will enforce name requirements.
    //        if (For.Metadata == null)
    //        {
    //            //throw new InvalidOperationException(Resources.FormatTagHelpers_NoProvidedMetadata(
    //            //    "<select>",
    //            //    ForAttributeName,
    //            //    nameof(IModelMetadataProvider),
    //            //    For.Name));
    //            throw new Exception("Metadata missing for RadioButtonList");
    //        }

    //        // Base allowMultiple on the instance or declared type of the expression to avoid a
    //        // "SelectExpressionNotEnumerable" InvalidOperationException during generation.
    //        // Metadata.IsEnumerableType is similar but does not take runtime type into account.
    //        //var realModelType = For.ModelExplorer.ModelType;
    //        //_allowMultiple = typeof(string) != realModelType &&
    //        //    typeof(IEnumerable).IsAssignableFrom(realModelType);
    //        _currentValue = Generator.GetCurrentValues(ViewContext, For.ModelExplorer, For.Name, false).FirstOrDefault();

    //        // Whether or not (not being highly unlikely) we generate anything, could update contained <option/>
    //        // elements. Provide selected values for <option/> tag helpers.
    //        //var currentValues = _currentValues == null ? null : new CurrentValues(_currentValues);
    //        context.Items[typeof(RadioButtonListHelper)] = _currentValue;
    //    }

    //    /// <inheritdoc />
    //    /// <remarks>Does nothing if <see cref="For"/> is <c>null</c>.</remarks>
    //    public override void Process(TagHelperContext context, TagHelperOutput output)
    //    {
    //        ArgumentNullException.ThrowIfNull(context);
    //        ArgumentNullException.ThrowIfNull(output);

    //        // Pass through attribute that is also a well-known HTML attribute. Must be done prior to any copying
    //        // from a TagBuilder.
    //        if (Name != null)
    //        {
    //            output.CopyHtmlAttribute(nameof(Name), context);
    //        }

    //        // Ensure GenerateSelect() _never_ looks anything up in ViewData.
    //        var items = Items ?? Enumerable.Empty<SelectListItem>();

    //        if (For == null)
    //        {
    //            var options = Generator.GenerateGroupsAndOptions(optionLabel: null, selectList: items);
    //            output.PostContent.AppendHtml(options);
    //            return;
    //        }

    //        // Ensure Generator does not throw due to empty "fullName" if user provided a name attribute.
    //        IDictionary<string, object> htmlAttributes = null;
    //        if (string.IsNullOrEmpty(For.Name) &&
    //            string.IsNullOrEmpty(ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix) &&
    //            !string.IsNullOrEmpty(Name))
    //        {
    //            htmlAttributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
    //            {
    //                { "name", Name },
    //            };
    //        }

    //        var tagBuilder = exGenerator.GenerateRadioList(
    //            ViewContext,
    //            For.ModelExplorer,
    //            optionLabel: null,
    //            expression: For.Name,
    //            selectList: items,
    //            htmlAttributes: htmlAttributes);

    //        if (tagBuilder != null)
    //        {
    //            output.MergeAttributes(tagBuilder);
    //            if (tagBuilder.HasInnerHtml)
    //            {
    //                output.PostContent.AppendHtml(tagBuilder.InnerHtml);
    //            }
    //        }
    //    }


    //}
}
