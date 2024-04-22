using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MultiLanguageProvider.AppCode.Extensions;
using MultiLanguageProvider.AppCode.Providers;
using TestEnvironment.Models.DataContext;
using TestEnvironment.Models.Entities;

namespace MultiLanguageProvider.Business.FaqModule
{
    public class FaqCreateCommand : FaqViewModel, IRequest<int>
    {
        public class FaqCreateCommandHandler : IRequestHandler<FaqCreateCommand, int>
        {
            private readonly LanguageProviderDbContext _dbContext;
            private readonly IActionContextAccessor _actionContextAccessor;
            public FaqCreateCommandHandler(LanguageProviderDbContext dbContext, IActionContextAccessor actionContextAccessor)
            {
                _dbContext = dbContext;
                _actionContextAccessor = actionContextAccessor;
            }
            public async Task<int> Handle(FaqCreateCommand request, CancellationToken cancellationToken)
            {
                if (!_actionContextAccessor.IsValidModelState())
                    return 0;
                else
                {
                    var entity = AutoMapper.Map<Faq>(request);
                    await _dbContext.Faqs.AddAsync(entity, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    return entity.Id;
                }
            }
        }
    }
}
