using Dsw2025Tpi.Domain.Entities;
using FluentValidation;

namespace Dsw2025Tpi.Application.Validators;

public class OrderItemValidator : AbstractValidator<OrderItem>
{
    public OrderItemValidator()
    {
        RuleFor(oi => oi.Quantity)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.");

        RuleFor(oi => oi.UnitPrice)
            .GreaterThan(0).WithMessage("El precio unitario debe ser mayor a 0.");
    }
}
