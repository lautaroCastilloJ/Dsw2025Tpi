using Dsw2025Tpi.Application.Dtos.Requests;
using FluentValidation;

namespace Dsw2025Tpi.Application.Validators;

public class ProductRequestValidator : AbstractValidator<ProductRequest>
{
    public ProductRequestValidator()
    {
        RuleFor(p => p.Sku)
            .NotEmpty().WithMessage("El SKU es obligatorio.");

        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.");

        RuleFor(p => p.InternalCode)
            .NotEmpty().WithMessage("El código interno es obligatorio.");

        RuleFor(p => p.CurrentUnitPrice)
            .GreaterThan(0).WithMessage("El precio debe ser mayor a 0.");

    }
}
