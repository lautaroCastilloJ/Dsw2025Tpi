using FluentValidation;
using Dsw2025Tpi.Application.Dtos.Requests;

namespace Dsw2025Tpi.Application.Validators;
public class OrderRequestValidator : AbstractValidator<OrderRequest>
{
    public OrderRequestValidator()
    {
        RuleFor(o => o.CustomerId).NotEmpty().WithMessage("El cliente es obligatorio.");
        RuleFor(o => o.ShippingAddress).NotEmpty().WithMessage("La dirección de envío es obligatoria.");
        RuleFor(o => o.BillingAddress).NotEmpty().WithMessage("La dirección de facturación es obligatoria.");
        RuleFor(o => o.OrderItems)
            .NotEmpty().WithMessage("La orden debe tener al menos un ítem.")
            .ForEach(item => item.SetValidator(new OrderItemRequestValidator()));
    }
}
