using FluentValidation;

namespace Bcp.Exchange.Application.Features.Parameters.Queries.GetParametersByParentCode;

public class GetParametersByParentCodeQueryValidator
    : AbstractValidator<GetParametersByParentCodeQuery>
{
    public GetParametersByParentCodeQueryValidator()
    {
        RuleFor(x => x.ParentCode)
            .NotEmpty()
            .WithMessage("El código del parámetro padre es requerido")
            .MaximumLength(50)
            .WithMessage("El código del parámetro padre no puede exceder 50 caracteres");
    }
}
