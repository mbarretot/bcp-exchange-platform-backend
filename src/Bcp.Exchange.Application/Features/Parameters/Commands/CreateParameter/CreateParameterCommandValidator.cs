using FluentValidation;

namespace Bcp.Exchange.Application.Features.Parameters.Commands.CreateParameter;

public class CreateParameterCommandValidator : AbstractValidator<CreateParameterCommand>
{
    public CreateParameterCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("El código es requerido")
            .MaximumLength(50)
            .WithMessage("El código no puede exceder 50 caracteres");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("La descripción es requerida")
            .MaximumLength(200)
            .WithMessage("La descripción no puede exceder 200 caracteres");

        RuleFor(x => x.LongDescription)
            .MaximumLength(500)
            .WithMessage("La descripción larga no puede exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.LongDescription));

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El orden debe ser mayor o igual a 0");

        RuleFor(x => x.NumericValue)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El valor numérico debe ser mayor o igual a 0")
            .When(x => x.NumericValue.HasValue);

        RuleFor(x => x.TextValue)
            .MaximumLength(500)
            .WithMessage("El valor de texto no puede exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.TextValue));

        RuleFor(x => x.CreatedBy)
            .NotEmpty()
            .WithMessage("El usuario creador es requerido")
            .MaximumLength(100)
            .WithMessage("El usuario creador no puede exceder 100 caracteres");
    }
}
