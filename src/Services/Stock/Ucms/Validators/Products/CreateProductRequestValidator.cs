namespace Ucms.Stock.Api.Validators.Products;

using FluentValidation;
using Ucms.Stock.Contracts.Requests.Products;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.NameRu).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
    }
}
