using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace MultiLanguageProvider.AppCode.Extensions
{
    public static partial class Extension
    {
        public static bool IsValidModelState(this IActionContextAccessor _actionContextAccessor)
        {
            return _actionContextAccessor.ActionContext!.ModelState.IsValid;
        }
    }
}
