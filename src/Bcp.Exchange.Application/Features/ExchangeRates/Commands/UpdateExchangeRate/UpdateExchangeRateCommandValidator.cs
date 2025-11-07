using FluentValidation;

namespace Bcp.Exchange.Application.Features.ExchangeRates.Commands.UpdateExchangeRate;

public class UpdateExchangeRateCommandValidator : AbstractValidator<UpdateExchangeRateCommand>
{
    public UpdateExchangeRateCommandValidator()
    {
        RuleFor(x => x.ExchangeRateId)
            .NotEmpty()
            .WithMessage("El ID de la tasa de cambio es requerido");

        RuleFor(x => x.Rate)
            .GreaterThan(0)
            .WithMessage("La tasa de cambio debe ser mayor a 0")
            .When(x => x.Rate.HasValue);

        RuleFor(x => x.CurrencySourceId)
            .NotEmpty()
            .WithMessage("El ID de la moneda origen es requerido")
            .When(x => x.CurrencySourceId.HasValue);

        RuleFor(x => x.CurrencyTargetId)
            .NotEmpty()
            .WithMessage("El ID de la moneda destino es requerido")
            .When(x => x.CurrencyTargetId.HasValue);

        RuleFor(x => x)
            .Must(x => x.CurrencySourceId != x.CurrencyTargetId)
            .WithMessage("La moneda origen y destino no pueden ser iguales")
            .When(x => x.CurrencySourceId.HasValue && x.CurrencyTargetId.HasValue);

        RuleFor(x => x.ModifiedBy)
            .NotEmpty()
            .WithMessage("El usuario modificador es requerido")
            .MaximumLength(100)
            .WithMessage("El usuario modificador no puede exceder 100 caracteres");
    }
}
