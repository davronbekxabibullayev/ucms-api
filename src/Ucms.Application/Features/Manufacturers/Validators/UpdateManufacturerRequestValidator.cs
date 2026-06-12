namespace Ucms.Application.Features.Manufacturers;

using FluentValidation;

public class UpdateManufacturerRequestValidator : AbstractValidator<UpdateManufacturer.Command>
{
    public UpdateManufacturerRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.NameRu).NotEmpty();
    }
}
