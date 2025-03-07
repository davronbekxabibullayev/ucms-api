namespace Ucms.Stock.Api.Validators.Products;

using FluentValidation;
using Ucms.Stock.Contracts.Requests.Products;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.NameRu).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
    }
}
