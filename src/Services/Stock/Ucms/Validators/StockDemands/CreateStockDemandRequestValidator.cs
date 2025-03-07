namespace Ucms.Stock.Api.Validators.StockDemands;

using FluentValidation;
using Ucms.Stock.Contracts.Requests.StockDemands;

public class CreateStockDemandRequestValidator : AbstractValidator<CreateStockDemandRequest>
{
    public CreateStockDemandRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.DemandDate).NotEmpty();
        RuleFor(x => x.SenderId).NotEmpty();
        RuleFor(x => x.RecipientId).NotEmpty();
        RuleForEach(x => x.Items).SetValidator(new CreateStockDemandItemModelValidator());
    }
}

public class CreateStockDemandItemModelValidator : AbstractValidator<CreateStockDemandItemModel>
{
    public CreateStockDemandItemModelValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.MeasurementUnitId).NotEmpty();
    }
}
