using MediatR;
using Microsoft.EntityFrameworkCore;
using MultiLanguageProvider.AppCode.Infrastructure;
using TestEnvironment.Models.DataContext;
using TestEnvironment.Models.Entities;

namespace MultiLanguageProvider.Business.FaqModule
{
    public class FaqRemoveCommand : IRequest<CommandJsonResponse>
    {
        public int Id { get; set; }
        public class FaqRemoveCommandHandler : IRequestHandler<FaqRemoveCommand, CommandJsonResponse>
        {
            private readonly LanguageProviderDbContext _dbContext;
            public FaqRemoveCommandHandler(LanguageProviderDbContext dbContext)
            {
                _dbContext = dbContext;
            }
            public async Task<CommandJsonResponse> Handle(FaqRemoveCommand request, CancellationToken cancellationToken)
            {
                CommandJsonResponse jsonResponse = new();
                if (!Helper.IsValidEntityId(request.Id))
                {
                    jsonResponse.SetErrorResponse("Invalid Faq ID");
                    return jsonResponse;
                }

                Faq? faq = await _dbContext.Faqs
                    .FirstOrDefaultAsync(academic => academic.Id == request.Id, cancellationToken);
                if (faq is null)
                {
                    jsonResponse.SetErrorResponse("Faq was not found");
                    return jsonResponse;
                }

                faq.DeletedTime = DateTime.Now;
                await _dbContext.SaveChangesAsync(cancellationToken);

                jsonResponse.StatusMessage = "Faq has been deleted successfully.";
                return jsonResponse;
            }
        }
    }
}