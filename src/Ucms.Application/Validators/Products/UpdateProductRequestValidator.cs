namespace Ucms.Application.Validators.Products;

using FluentValidation;
using Ucms.Application.Features.Products;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProduct.Command>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.NameRu).NotEmpty();
    }
}
