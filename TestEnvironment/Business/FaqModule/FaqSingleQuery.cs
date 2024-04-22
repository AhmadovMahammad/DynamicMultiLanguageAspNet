using MediatR;
using Microsoft.EntityFrameworkCore;
using TestEnvironment.Models.DataContext;
using TestEnvironment.Models.Entities;

namespace MultiLanguageProvider.Business.FaqModule
{
    public class FaqSingleQuery : IRequest<Faq>
    {
        public int Id { get; set; }
        public class FaqSingleQueryHandler : IRequestHandler<FaqSingleQuery, Faq>
        {
            private readonly LanguageProviderDbContext _dbContext;
            public FaqSingleQueryHandler(LanguageProviderDbContext dbContext)
            {
                _dbContext = dbContext;
            }
            public async Task<Faq> Handle(FaqSingleQuery request, CancellationToken cancellationToken)
            {
                Faq? entity = await _dbContext.Faqs.FirstOrDefaultAsync(m => m.Id == request.Id && m.DeletedTime == null, cancellationToken);
                return entity ?? throw new InvalidOperationException("Faq could not find"); ;
            }
        }
    }
}
