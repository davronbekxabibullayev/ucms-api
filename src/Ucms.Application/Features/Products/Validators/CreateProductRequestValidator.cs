namespace Ucms.Application.Features.Products;

using FluentValidation;

public class CreateProductRequestValidator : AbstractValidator<CreateProduct.Command>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.NameRu).NotEmpty();
    }
}
