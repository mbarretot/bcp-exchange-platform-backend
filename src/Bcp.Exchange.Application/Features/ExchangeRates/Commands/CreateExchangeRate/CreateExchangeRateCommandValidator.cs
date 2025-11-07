using FluentValidation;

namespace Bcp.Exchange.Application.Features.ExchangeRates.Commands.CreateExchangeRate;

public class CreateExchangeRateCommandValidator : AbstractValidator<CreateExchangeRateCommand>
{
    public CreateExchangeRateCommandValidator()
    {
        RuleFor(x => x.Rate).GreaterThan(0).WithMessage("La tasa de cambio debe ser mayor a 0");

        RuleFor(x => x.CurrencySourceId)
            .NotEmpty()
            .WithMessage("El ID de la moneda origen es requerido");

        RuleFor(x => x.CurrencyTargetId)
            .NotEmpty()
            .WithMessage("El ID de la moneda destino es requerido");

        RuleFor(x => x)
            .Must(x => x.CurrencySourceId != x.CurrencyTargetId)
            .WithMessage("La moneda origen y destino no pueden ser iguales")
            .When(x => x.CurrencySourceId != Guid.Empty && x.CurrencyTargetId != Guid.Empty);

        RuleFor(x => x.CreatedBy)
            .NotEmpty()
            .WithMessage("El usuario creador es requerido")
            .MaximumLength(100)
            .WithMessage("El usuario creador no puede exceder 100 caracteres");
    }
}
