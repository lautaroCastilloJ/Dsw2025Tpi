using Dsw2025Tpi.Application.Dtos.Orders;
using FluentValidation;

namespace Dsw2025Tpi.Application.Validators;

public sealed class OrderRequestValidator : AbstractValidator<OrderRequest>
{
    public OrderRequestValidator()
    {
        RuleFor(o => o.CustomerId)
            .NotEmpty().WithMessage("El identificador del cliente es obligatorio.");

        RuleFor(o => o.ShippingAddress)
            .NotEmpty().WithMessage("La dirección de envío es obligatoria.")
            .MaximumLength(100).WithMessage("La dirección de envío no puede exceder 100 caracteres.");

        RuleFor(o => o.BillingAddress)
            .NotEmpty().WithMessage("La dirección de facturación es obligatoria.")
            .MaximumLength(100).WithMessage("La dirección de facturación no puede exceder 100 caracteres.");

        RuleFor(o => o.Notes)
            .MaximumLength(300)
            .When(o => o.Notes is not null)
            .WithMessage("Las notas no pueden exceder 300 caracteres.");

        RuleFor(o => o.OrderItems)
            .NotNull().WithMessage("La orden debe incluir al menos un ítem.")
            .Must(items => items.Any())
            .WithMessage("La orden debe incluir al menos un ítem.");

        RuleForEach(o => o.OrderItems)
            .SetValidator(new OrderItemRequestValidator());
    }
}
