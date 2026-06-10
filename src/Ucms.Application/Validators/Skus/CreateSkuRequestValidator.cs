namespace Ucms.Application.Validators.Skus;

using FluentValidation;
using Ucms.Application.DTOs.Requests.Skus;

public class CreateSkuRequestValidator : AbstractValidator<CreateSkuRequest>
{
    public CreateSkuRequestValidator()
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
