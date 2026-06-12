namespace Ucms.Application.Validators.Manufacturers;

using FluentValidation;
using Ucms.Application.Features.Manufacturers;

public class CreateManufacturerRequestValidator : AbstractValidator<CreateManufacturer.Command>
{
    public CreateManufacturerRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.NameRu).NotEmpty();
    }
}
