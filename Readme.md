# Multi-Language Dynamic Data Handling in ASP.NET

## Introduction
This repository contains an implementation for handling dynamic multi-language data in ASP.NET. While multi-language support for static data is common using resources files, this project extends that capability to dynamic data.
### For detailed information about extension, follow this path -> TestEnvironment/AppCode/Extensions/MultiLanguageExtension.cs

## Model Definition
To utilize this functionality, your model should inherit from `IMultiLanguage` interface and use the `LocalizedPropertyAttribute`.

```csharp
public class FaqViewModel : IMultiLanguage
{
    // Properties for the default language (Azerbaijani)
    [LocalizedProperty]
    public string Question { get; set; } = string.Empty;
    [LocalizedProperty]
    public string Answer { get; set; } = string.Empty;

    // Properties for additional language (English)
    [LocalizedProperty]
    public string QuestionEng { get; set; } = string.Empty;
    [LocalizedProperty]
    public string AnswerEng { get; set; } = string.Empty;

    public int Id { get; set; }
}
```

## Admin-Side Integration (CRUD Operations)

##### CRUD operations can be performed on the dynamic data using methods provided by the LanguageProvider.

```csharp
[Area("Admin")]
public class FaqController : Controller
{
    private readonly LanguageProvider _langProvider = new("faq", "FaqResource");
    public FaqsController()
    {
        _langProvider.CreateDirectory();
    }
}
```

``` csharp
// Write dynamic data pairs
_langProvider.WritePairs(model);

// Update dynamic data pairs
_langProvider.UpdatePairs(id, model);

// Remove dynamic data pairs
_langProvider.RemovePairs(id);
```

## Client-Side Integration
The dynamic data can be accessed in the client-side controller and displayed in the view.

```csharp
public class FaqController : Controller
{
    private readonly LanguageProvider _languageProviderFaq = new("faq", "FaqResource");
    public FaqController(){
    }
}
```

```csharp
// Read dynamic data based on current language
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
```

```html
<!-- View -->
@foreach (var item in Model.Select((value, index) => new { index, value }))
{
    <tr>
        <td>@faqs?[item.index]["Question"]</td>
        <td>@faqs?[item.index]["Answer"]</td>
    </tr>
}
```
