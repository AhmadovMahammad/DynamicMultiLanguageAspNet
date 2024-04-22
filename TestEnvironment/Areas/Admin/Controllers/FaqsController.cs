using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiLanguageProvider.AppCode.Extensions;
using MultiLanguageProvider.AppCode.Infrastructure;
using MultiLanguageProvider.AppCode.Providers;
using MultiLanguageProvider.Business.FaqModule;
using System.Reflection.Emit;
using TestEnvironment.Models.DataContext;
using TestEnvironment.Models.Entities;

namespace TestEnvironment.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AllowAnonymous]
    public class FaqsController : Controller
    {
        private readonly LanguageProviderDbContext _dbContext;
        private readonly IMediator _mediator;
        private readonly LanguageProvider _langProvider = new("faq", "FaqResource");
        public FaqsController(LanguageProviderDbContext braInvestDbContext, IMediator mediator)
        {
            _dbContext = braInvestDbContext;
            _mediator = mediator;
            _langProvider.CreateDirectory();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Faq>? response = await _dbContext.Faqs
                .Where(m => m.DeletedTime == null)
                .ToListAsync();
            return response is null ? NotFound() : View(response);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FaqCreateCommand command)
        {
            if (ModelState.IsValid)
            {
                int response = await _mediator.Send(command);
                if (response > 0)
                {
                    //add the chosen faq into both database and json
                    command.Id = response;
                    _langProvider.WritePairs(command);
                    return RedirectToAction(nameof(Index));
                }
            }
            ModelState.AddModelError("", "Error occured while creating Faq!");
            return View(command);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(FaqSingleQuery query)
        {
            Faq faq;
            try
            {
                faq = await _mediator.Send(query);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Error");
            }
            return View(AutoMapper.Map<FaqEditCommand>(faq));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FaqEditCommand command)
        {

            int response = await _mediator.Send(command);
            if (response > 0)
            {
                //update the chosen faq in both database and json
                _langProvider.UpdatePairs(response, command);
                return RedirectToAction(nameof(Index));
            }
            return View(command);
        }

        [HttpPost]
        public async Task<IActionResult> Remove(FaqRemoveCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.HasError)
                return Json(response);

            //remove the chosen faq from both database and json
            _langProvider.RemovePairs(command.Id);
            List<Faq> faqs = await _dbContext.Faqs.Where(m => m.DeletedTime == null).ToListAsync();
            return PartialView("_ListBody", faqs);
        }
    }
}
