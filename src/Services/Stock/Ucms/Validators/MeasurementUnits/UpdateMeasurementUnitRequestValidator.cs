namespace Ucms.Stock.Api.Validators.MeasurementUnits;

using FluentValidation;
using Ucms.Stock.Contracts.Requests.MeasurementUnits;

public class UpdateMeasurementUnitRequestValidator : AbstractValidator<UpdateMeasurementUnitRequest>
{
    public UpdateMeasurementUnitRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.NameRu).NotEmpty();
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.Multiplier).GreaterThan(0);
    }
}
