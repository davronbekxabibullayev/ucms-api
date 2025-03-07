namespace Ucms.Stock.Api.Validators.Incomes;

using FluentValidation;
using Ucms.Stock.Domain.Models.Enums;
using Ucms.Stock.Contracts.Requests.Incomes;

public class UpdateOutcomeRequestValidator : AbstractValidator<UpdateOutcomeRequest>
{
    public UpdateOutcomeRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.StockId).NotEmpty();
        RuleFor(x => x.OutcomeDate).NotEmpty();
        RuleFor(x => x.OutcomeType).NotEmpty();
        RuleFor(x => x.PaymentType).NotEmpty();
        RuleFor(x => x.OutcomeStatus).NotEmpty();
        RuleFor(x => x.IncomeStockId).NotEmpty().When(w => w.OutcomeType is OutcomeType.Broadcast or OutcomeType.Return);
        RuleForEach(x => x.OutcomeItems).SetValidator(new UpdateOutcomeItemModelValidator());
    }
}

public class UpdateOutcomeItemModelValidator : AbstractValidator<UpdateOutcomeItemModel>
{
    public UpdateOutcomeItemModelValidator()
    {
        RuleFor(x => x.SkuId).NotEmpty();
        RuleFor(x => x.MeasurementUnitId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}
