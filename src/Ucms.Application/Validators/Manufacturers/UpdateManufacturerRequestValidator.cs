namespace Ucms.Application.Validators.Manufacturers;

using FluentValidation;
using Ucms.Application.Features.Manufacturers;

public class UpdateManufacturerRequestValidator : AbstractValidator<UpdateManufacturer.Command>
{
    public UpdateManufacturerRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.NameRu).NotEmpty();
    }
}
