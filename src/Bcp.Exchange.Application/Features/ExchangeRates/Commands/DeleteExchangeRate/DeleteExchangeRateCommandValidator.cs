using FluentValidation;

namespace Bcp.Exchange.Application.Features.ExchangeRates.Commands.DeleteExchangeRate;

public class DeleteExchangeRateCommandValidator : AbstractValidator<DeleteExchangeRateCommand>
{
    public DeleteExchangeRateCommandValidator()
    {
        RuleFor(x => x.ExchangeRateId)
            .NotEmpty()
            .WithMessage("El ID de la tasa de cambio es requerido");

        RuleFor(x => x.ModifiedBy)
            .NotEmpty()
            .WithMessage("El usuario modificador es requerido")
            .MaximumLength(100)
            .WithMessage("El usuario modificador no puede exceder 100 caracteres");
    }
}
