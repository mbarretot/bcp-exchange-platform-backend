using FluentValidation;

namespace Bcp.Exchange.Application.Features.Parameters.Commands.UpdateParameter;

public class UpdateParameterCommandValidator : AbstractValidator<UpdateParameterCommand>
{
    public UpdateParameterCommandValidator()
    {
        RuleFor(x => x.ParameterId).NotEmpty().WithMessage("El ID del parámetro es requerido");

        RuleFor(x => x.Description)
            .MaximumLength(200)
            .WithMessage("La descripción no puede exceder 200 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.LongDescription)
            .MaximumLength(500)
            .WithMessage("La descripción larga no puede exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.LongDescription));

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El orden debe ser mayor o igual a 0")
            .When(x => x.DisplayOrder.HasValue);

        RuleFor(x => x.NumericValue)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El valor numérico debe ser mayor o igual a 0")
            .When(x => x.NumericValue.HasValue);

        RuleFor(x => x.TextValue)
            .MaximumLength(500)
            .WithMessage("El valor de texto no puede exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.TextValue));

        RuleFor(x => x.ModifiedBy)
            .NotEmpty()
            .WithMessage("El usuario modificador es requerido")
            .MaximumLength(100)
            .WithMessage("El usuario modificador no puede exceder 100 caracteres");
    }
}
