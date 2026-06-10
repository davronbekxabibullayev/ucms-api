namespace Ucms.Stock.Api.Validators.Incomes;

using FluentValidation;
using Ucms.Stock.Contracts.Requests.Incomes;

public class UpdateIncomeRequestValidator : AbstractValidator<UpdateIncomeRequest>
{
    public UpdateIncomeRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.StockId).NotEmpty();
        RuleFor(x => x.IncomeDate).NotEmpty();
        RuleFor(x => x.IncomeType).NotEmpty();
        RuleFor(x => x.PaymentType).NotEmpty();
        RuleFor(x => x.IncomeStatus).NotEmpty();
        RuleForEach(x => x.IncomeItems).SetValidator(new UpdateIncomeItemModelValidator());
    }
}

public class UpdateIncomeItemModelValidator : AbstractValidator<UpdateIncomeItemModel>
{
    public UpdateIncomeItemModelValidator()
    {
        RuleFor(x => x.SkuId).NotEmpty();
        RuleFor(x => x.MeasurementUnitId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}
