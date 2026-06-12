namespace Ucms.Application.Features.Manufacturers;

using FluentValidation;

public class CreateManufacturerRequestValidator : AbstractValidator<CreateManufacturer.Command>
{
    public CreateManufacturerRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.NameRu).NotEmpty();
    }
}
