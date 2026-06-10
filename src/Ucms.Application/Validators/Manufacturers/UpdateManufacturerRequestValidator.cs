namespace Ucms.Application.Validators.Manufacturers;

using FluentValidation;
using Ucms.Application.DTOs.Requests.Manufacturers;

public class UpdateManufacturerRequestValidator : AbstractValidator<UpdateManufacturerRequest>
{
    public UpdateManufacturerRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.NameRu).NotEmpty();
    }
}
