using Microsoft.AspNetCore.Localization;
using System.Text.RegularExpressions;

namespace MultiLanguageProvider.AppCode.Providers
{
    public class CultureProvider : RequestCultureProvider
    {
        public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
        {
            PathString path = httpContext.Request.Path;
            Match match = Regex.Match(path, @"\/(?<language>az|en)\/?.*");
            string chosenLanguage = string.Empty;

            //if user entered language from url
            if (match.Success)
            {
                chosenLanguage = match.Groups["language"].Value;

                //remove previous cookies for language options
                httpContext.Response.Cookies.Delete("chosenLanguage");
                httpContext.Response.Cookies.Append("chosenLanguage", chosenLanguage, new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(7)
                });

                return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(chosenLanguage, chosenLanguage));
            }

            //looking at cookies
            if (httpContext.Request.Cookies.TryGetValue("chosenLanguage", out string? language))
            {
                if (!string.IsNullOrWhiteSpace(language))
                    chosenLanguage = language;
                return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(chosenLanguage, chosenLanguage));
            }

            //otherwise add default language
            httpContext.Response.Cookies.Append("chosenLanguage", "en", new CookieOptions { Expires = DateTime.Now.AddDays(7) });
            return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(chosenLanguage, chosenLanguage));
        }
    }
}
