using MultiLanguageProvider.AppCode.Infrastructure;

namespace MultiLanguageProvider.Business
{
    public static class Helper
    {
        public static bool IsValidEntityId(int? workerId)
        {
            return workerId.HasValue && workerId > 0;
        }
        public static void SetErrorResponse(this CommandJsonResponse response, string message)
        {
            response.HasError = true;
            response.StatusMessage = message;
        }
    }
}
