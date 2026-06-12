namespace Ucms.Application.Validators.Products;

using FluentValidation;
using Ucms.Application.Features.Products;

public class CreateProductRequestValidator : AbstractValidator<CreateProduct.Command>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.NameRu).NotEmpty();
    }
}
