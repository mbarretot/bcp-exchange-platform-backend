namespace Bcp.Exchange.Application.Common.Exceptions;

public class ValidationException : Exception
{
    public IEnumerable<ValidationError> Errors { get; }

    public ValidationException(IEnumerable<ValidationError> errors)
        : base("Se encontraron uno o más errores de validación")
    {
        Errors = errors;
    }

    public ValidationException(string propertyName, string errorMessage)
        : base("Se encontraron uno o más errores de validación")
    {
        Errors = new[] { new ValidationError(propertyName, errorMessage) };
    }
}

public record ValidationError(string PropertyName, string ErrorMessage);
