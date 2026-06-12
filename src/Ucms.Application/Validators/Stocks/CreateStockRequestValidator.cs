namespace Ucms.Application.Validators.Stocks;

using FluentValidation;
using Ucms.Application.Features.Stocks;

public class CreateStockRequestValidator : AbstractValidator<CreateStock.Command>
{
    public CreateStockRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.NameRu).NotEmpty();
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.OrganizationId).NotEmpty();
    }
}
