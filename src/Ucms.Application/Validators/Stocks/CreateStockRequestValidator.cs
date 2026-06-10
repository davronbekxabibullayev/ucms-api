namespace Ucms.Application.Validators.Stocks;

using FluentValidation;
using Ucms.Application.DTOs.Requests.Stocks;

public class CreateStockRequestValidator : AbstractValidator<CreateStockRequest>
{
    public CreateStockRequestValidator()
    {
        RuleFor(x => x.NameRu).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Code ).NotEmpty();
        RuleFor(x => x.OrganizationId).NotEmpty();
    }
}
