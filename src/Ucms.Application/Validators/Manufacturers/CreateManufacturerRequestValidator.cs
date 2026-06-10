namespace Ucms.Application.Validators.Manufacturers;

using FluentValidation;
using Ucms.Application.DTOs.Requests.Manufacturers;

public class CreateManufacturerRequestValidator : AbstractValidator<CreateManufacturerRequest>
{
    public CreateManufacturerRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.NameRu).NotEmpty();
    }
}
