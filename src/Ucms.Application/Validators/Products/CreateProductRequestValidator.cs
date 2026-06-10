namespace Ucms.Application.Validators.Products;

using FluentValidation;
using Ucms.Application.DTOs.Requests.Products;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.NameRu).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
    }
}
