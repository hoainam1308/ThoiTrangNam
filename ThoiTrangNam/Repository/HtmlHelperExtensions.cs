using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

public static class HtmlHelperExtensions
{
    public static HtmlString EmbedMedia(this IHtmlHelper htmlHelper, string description)
    {
        if (string.IsNullOrEmpty(description))
        {
            return new HtmlString(description);
        }

        // Xóa thẻ oembed trước khi nhúng iframe
        description = Regex.Replace(description, @"<oembed[^>]*url=""([^""]*)""[^>]*><\/oembed>", m =>
        {
            string url = m.Groups[1].Value;
            return $"<iframe width=\"560\" height=\"315\" src=\"{url}\" frameborder=\"0\" allowfullscreen></iframe>";
        });

        return new HtmlString(description);
    }

}
