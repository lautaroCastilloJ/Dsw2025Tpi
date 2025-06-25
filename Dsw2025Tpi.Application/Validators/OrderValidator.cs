using Dsw2025Tpi.Domain.Entities;
using FluentValidation;

namespace Dsw2025Tpi.Application.Validators;

public class OrderValidator : AbstractValidator<Order>
{
    public OrderValidator()
    {
        RuleFor(o => o.CustomerId)
            .NotEmpty().WithMessage("El cliente es obligatorio.");

        RuleFor(o => o.ShippingAddress)
            .NotEmpty().WithMessage("La dirección de envío es obligatoria.");

        RuleFor(o => o.BillingAddress)
            .NotEmpty().WithMessage("La dirección de facturación es obligatoria.");

        RuleFor(o => o.OrderItems)
            .NotNull().WithMessage("Debe haber al menos un ítem en la orden.")
            .Must(items => items != null && items.Count > 0).WithMessage("Debe haber al menos un ítem en la orden.");
    }
}
