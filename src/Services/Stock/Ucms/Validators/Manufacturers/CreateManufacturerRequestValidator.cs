namespace Ucms.Stock.Api.Validators.Manufacturers;

using FluentValidation;
using Ucms.Stock.Contracts.Requests.Manufacturers;

public class CreateManufacturerRequestValidator : AbstractValidator<CreateManufacturerRequest>
{
    public CreateManufacturerRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.NameRu).NotEmpty();
    }
}
