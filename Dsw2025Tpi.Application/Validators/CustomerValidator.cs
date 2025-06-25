using Dsw2025Tpi.Domain.Entities;
using FluentValidation;

namespace Dsw2025Tpi.Application.Validators;

// Opcional
public class CustomerValidator : AbstractValidator<Customer>
{
    public CustomerValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.");

        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.");
    }
}
