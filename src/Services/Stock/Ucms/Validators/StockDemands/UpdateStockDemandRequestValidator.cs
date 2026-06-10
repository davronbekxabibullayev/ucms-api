namespace Ucms.Stock.Api.Validators.StockDemands;

using FluentValidation;
using Ucms.Stock.Contracts.Requests.StockDemands;

public class UpdateStockDemandRequestValidator : AbstractValidator<UpdateStockDemandRequest>
{
    public UpdateStockDemandRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.DemandDate).NotEmpty();
        RuleFor(x => x.SenderId).NotEmpty();
        RuleFor(x => x.RecipientId).NotEmpty();
        RuleForEach(x => x.Items).SetValidator(new UpdateStockDemandItemModelValidator());
    }
}

public class UpdateStockDemandItemModelValidator : AbstractValidator<UpdateStockDemandItemModel>
{
    public UpdateStockDemandItemModelValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.MeasurementUnitId).NotEmpty();
    }
}
