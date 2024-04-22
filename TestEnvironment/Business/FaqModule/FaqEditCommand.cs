using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MultiLanguageProvider.AppCode.Extensions;
using TestEnvironment.Models.DataContext;
using TestEnvironment.Models.Entities;

namespace MultiLanguageProvider.Business.FaqModule
{
    public class FaqEditCommand : FaqViewModel, IRequest<int>
    {
        public class FaqEditCommandHandler : IRequestHandler<FaqEditCommand, int>
        {
            private readonly LanguageProviderDbContext _dbContext;
            private readonly IActionContextAccessor _actionContextAccessor;
            public FaqEditCommandHandler(LanguageProviderDbContext dbContext, IActionContextAccessor actionContextAccessor)
            {
                _dbContext = dbContext;
                _actionContextAccessor = actionContextAccessor;
            }
            public async Task<int> Handle(FaqEditCommand request, CancellationToken cancellationToken)
            {
                if (request.Id <= 0)
                    return 0;
                Faq? faq = await _dbContext.Faqs
                    .FirstOrDefaultAsync(b => b.Id == request.Id && b.DeletedTime == null, cancellationToken) ?? throw new InvalidOperationException("Faq could not find"); ;
                if (faq is null)
                    return 0;

                if (_actionContextAccessor.IsValidModelState())
                {
                    faq.Question = request.Question;
                    faq.Answer = request.Answer;
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    return faq.Id;
                }
                return 0;
            }
        }
    }
}
