using System.Text.RegularExpressions;

namespace MultiLanguageProvider.AppCode.Extensions
{
    public static partial class Extension
    {
        public static string GetCurrentCulture(this HttpContext httpContext)
        {
            Match languageMatch = Regex.Match(httpContext.Request.Path, @"\/(?<lang>az|en)\/?.*", RegexOptions.IgnoreCase);
            if (languageMatch.Success)
                return languageMatch.Groups["lang"].Value ?? "en";

            if (httpContext.Request.Cookies.TryGetValue("chosenLanguage", out string? language))
            {
                language = language?.Trim().ToLowerInvariant();
                return !string.IsNullOrEmpty(language) ? language : "en";
            }

            return "en";
        }
    }
}
