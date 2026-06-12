namespace Ucms.Application.Validators.Incomes;

using FluentValidation;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Features.Incomes;

public class CreateIncomeRequestValidator : AbstractValidator<CreateIncome.Command>
{
    public CreateIncomeRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.StockId).NotEmpty();
        RuleFor(x => x.IncomeDate).NotEmpty();
        RuleFor(x => x.IncomeType).IsInEnum();
        RuleFor(x => x.PaymentType).IsInEnum();
        RuleFor(x => x.IncomeStatus).IsInEnum();
        RuleForEach(x => x.IncomeItems).SetValidator(new IncomeItemValidator());
    }
}

public class IncomeItemValidator : AbstractValidator<CreateIncomeItemModel>
{
    public IncomeItemValidator()
    {
        RuleFor(x => x.SkuId).NotEmpty();
        RuleFor(x => x.MeasurementUnitId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}
