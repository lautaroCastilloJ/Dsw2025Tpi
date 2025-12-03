using FluentValidation;
using Dsw2025Tpi.Application.Dtos.Payments;

namespace Dsw2025Tpi.Application.Validators;

public class CreatePaymentRequestValidator : AbstractValidator<CreatePaymentRequest>
{
    public CreatePaymentRequestValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("OrderId is required");

        RuleFor(x => x.BackUrl)
            .MaximumLength(500)
            .WithMessage("BackUrl cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.BackUrl));
    }
}
