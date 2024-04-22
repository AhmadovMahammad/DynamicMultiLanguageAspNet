using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiLanguageProvider.AppCode.Extensions;
using TestEnvironment.Models.DataContext;

namespace TestEnvironment.Controllers
{
    [AllowAnonymous]
    public class FaqController : Controller
    {
        private readonly LanguageProviderDbContext _context;
        private readonly LanguageProvider _languageProviderFaq = new("faq", "FaqResource");
        public FaqController(LanguageProviderDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string currentLanguage = HttpContext.GetCurrentCulture();
            List<Dictionary<string, string>>? jsonData = _languageProviderFaq.ReadFullJson(currentLanguage is "en" ? LanguageOptions.Eng : LanguageOptions.Aze);
            ViewData["Faqs"] = jsonData;

            var faqs = await _context.Faqs
                .Where(m => m.DeletedTime == null).ToListAsync();
            return View(faqs);
        }
    }
}
