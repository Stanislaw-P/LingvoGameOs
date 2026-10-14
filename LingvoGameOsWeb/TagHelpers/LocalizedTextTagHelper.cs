using LingvoGameOs.Helpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;

namespace LingvoGameOs.TagHelpers;

[HtmlTargetElement("localized-text", Attributes = "key")]
public sealed class LocalizedTextTagHelper(
    IStringLocalizer<SharedResource> primaryLocalizer,
    SecondLocalizer secondLocalizer) : TagHelper
{
    [HtmlAttributeName("key")]
    public string Key { get; set; } = string.Empty;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var primary = primaryLocalizer[Key].Value;
        var secondary = secondLocalizer[Key];

        output.TagName = "span";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("class", "localized-text");
        output.Content.SetContent(primary);

        if (secondLocalizer.Culture is null ||
            string.Equals(primary, secondary, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        output.Attributes.SetAttribute("data-secondary", secondary);
        output.Attributes.SetAttribute("tabindex", "0");
        output.Attributes.SetAttribute("aria-label", $"{primary}. {secondary}");
    }
}
