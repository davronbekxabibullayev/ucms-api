namespace Ucms.Application.Features.Products;

using FluentValidation;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProduct.Command>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.NameRu).NotEmpty();
    }
}
