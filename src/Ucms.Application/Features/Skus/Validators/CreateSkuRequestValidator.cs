namespace Ucms.Application.Features.Skus.Validators;

using FluentValidation;
using Ucms.Application.Features.Skus.Commands;

public class CreateSkuRequestValidator : AbstractValidator<CreateSku.Command>
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
