namespace Ucms.Stock.Api.Validators.Manufacturers;

using FluentValidation;
using Ucms.Stock.Contracts.Requests.Manufacturers;

public class UpdateManufacturerRequestValidator : AbstractValidator<UpdateManufacturerRequest>
{
    public UpdateManufacturerRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.NameRu).NotEmpty();
    }
}
