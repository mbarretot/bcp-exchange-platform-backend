using FluentValidation;

namespace Bcp.Exchange.Application.Features.Parameters.Commands.DeleteParameter;

public class DeleteParameterCommandValidator : AbstractValidator<DeleteParameterCommand>
{
    public DeleteParameterCommandValidator()
    {
        RuleFor(x => x.ParameterId).NotEmpty().WithMessage("El ID del parámetro es requerido");

        RuleFor(x => x.ModifiedBy)
            .NotEmpty()
            .WithMessage("El usuario que realiza la eliminación es requerido")
            .MaximumLength(100)
            .WithMessage("El nombre del usuario no puede exceder 100 caracteres");
    }
}
