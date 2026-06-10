namespace Ucms.Application.Validators.MeasurementUnits;

using FluentValidation;
using Ucms.Application.DTOs.Requests.MeasurementUnits;

public class CreateMeasurementUnitRequestValidator : AbstractValidator<CreateMeasurementUnitRequest>
{
    public CreateMeasurementUnitRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.NameRu).NotEmpty();
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.Multiplier).GreaterThan(0);
    }
}
