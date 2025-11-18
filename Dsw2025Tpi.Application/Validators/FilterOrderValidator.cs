using FluentValidation;

namespace Dsw2025Tpi.Application.Dtos.Orders;

public sealed class FilterOrderValidator : AbstractValidator<FilterOrder>
{
    public FilterOrderValidator()
    {
        RuleFor(f => f.PageNumber)
            .GreaterThan(0).When(f => f.PageNumber.HasValue)
            .WithMessage("El número de página debe ser mayor que 0.");

        RuleFor(f => f.PageSize)
            .GreaterThan(0).When(f => f.PageSize.HasValue)
            .WithMessage("El tamaño de página debe ser mayor que 0.")
            .LessThanOrEqualTo(100).When(f => f.PageSize.HasValue)
            .WithMessage("El tamaño máximo de página permitido es 100.");
    }
}
