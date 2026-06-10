namespace Ucms.Stock.Api.Validators.Incomes;

using FluentValidation;
using Ucms.Stock.Contracts.Requests.Incomes;

public class CreateIncomeRequestValidator : AbstractValidator<CreateIncomeRequest>
{
    public CreateIncomeRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.StockId).NotEmpty();
        RuleFor(x => x.IncomeDate).NotEmpty();
        RuleFor(x => x.IncomeType).NotEmpty();
        RuleFor(x => x.PaymentType).NotEmpty();
        RuleFor(x => x.IncomeStatus).NotEmpty();
        RuleForEach(x => x.IncomeItems).SetValidator(new CreateIncomeItemModelValidator());
    }
}

public class CreateIncomeItemModelValidator : AbstractValidator<CreateIncomeItemModel>
{
    public CreateIncomeItemModelValidator()
    {
        RuleFor(x => x.SkuId).NotEmpty();
        RuleFor(x => x.MeasurementUnitId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}
