using Dsw2025Tpi.Domain.Entities;
using FluentValidation;

namespace Dsw2025Tpi.Application.Validators;

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {

        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.");

        RuleFor(p => p.CurrentUnitPrice)
            .GreaterThan(0).WithMessage("El precio debe ser mayor a 0.");

        RuleFor(p => p.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("El stock no debe ser negativo.");
    }
}
