using MultiLanguageProvider.AppCode.Infrastructure;
using MultiLanguageProvider.AppCode.Providers;

namespace MultiLanguageProvider.Business.FaqModule
{
    public class FaqViewModel : IMultiLanguage
    {
        // Properties for the default language (Azerbaijani)
        [LocalizedProperty]
        public string Question { get; set; } = string.Empty;
        [LocalizedProperty]
        public string Answer { get; set; } = string.Empty;

        // Properties for the default language (English)
        [LocalizedProperty]
        public string QuestionEng { get; set; } = string.Empty;
        [LocalizedProperty]
        public string AnswerEng { get; set; } = string.Empty;

        public int Id { get; set; }
    }
}
