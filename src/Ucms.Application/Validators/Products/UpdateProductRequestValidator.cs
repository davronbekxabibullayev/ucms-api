namespace Ucms.Application.Validators.Products;

using FluentValidation;
using Ucms.Application.DTOs.Requests.Products;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.NameRu).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
    }
}
