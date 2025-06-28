using FluentValidation;
using Dsw2025Tpi.Application.Dtos.Requests;

namespace Dsw2025Tpi.Application.Validators;
public class OrderItemRequestValidator : AbstractValidator<OrderItemRequest>
{
    public OrderItemRequestValidator()
    {
        RuleFor(i => i.ProductId).NotEmpty().WithMessage("El ID del producto es obligatorio.");
        RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("La cantidad debe ser mayor a cero.");
    }
}
