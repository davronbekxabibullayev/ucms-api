namespace Ucms.Application.Validators.MeasurementUnits;

using FluentValidation;
using Ucms.Application.Features.MeasurementUnits;

public class CreateMeasurementUnitRequestValidator : AbstractValidator<CreateMeasurementUnit.Command>
{
    public CreateMeasurementUnitRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.NameRu).NotEmpty();
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.Multiplier).GreaterThan(0);
    }
}
