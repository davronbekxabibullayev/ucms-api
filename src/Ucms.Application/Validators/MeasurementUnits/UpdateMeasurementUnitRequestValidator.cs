namespace Ucms.Application.Validators.MeasurementUnits;

using FluentValidation;
using Ucms.Application.Features.MeasurementUnits;

public class UpdateMeasurementUnitRequestValidator : AbstractValidator<UpdateMeasurementUnit.Command>
{
    public UpdateMeasurementUnitRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.NameRu).NotEmpty();
        RuleFor(x => x.Multiplier).GreaterThan(0);
    }
}
