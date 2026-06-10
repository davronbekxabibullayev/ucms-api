namespace Ucms.Stock.Api.Validators.Skus;

using FluentValidation;
using Ucms.Stock.Contracts.Requests.Skus;

public class UpdateSkuRequestValidator : AbstractValidator<UpdateSkuRequest>
{
    public UpdateSkuRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.NameRu).NotEmpty();
        RuleFor(x => x.SerialNumber).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.MeasurementUnitId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
