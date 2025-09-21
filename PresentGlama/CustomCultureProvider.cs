using Microsoft.AspNetCore.Localization;

namespace PresentGlama
{
    public class CustomCultureProvider : RequestCultureProvider
    {
        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext Context)
        {
            var DefautlCulture = "en";
            var Culture = Context.Request.RouteValues["Culture"]?.ToString() ?? DefautlCulture;
            await Task.Yield();
            return new ProviderCultureResult(Culture);
        }
    }
}
