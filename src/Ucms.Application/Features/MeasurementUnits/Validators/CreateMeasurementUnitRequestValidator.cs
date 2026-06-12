namespace Ucms.Application.Features.MeasurementUnits;

using FluentValidation;

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
