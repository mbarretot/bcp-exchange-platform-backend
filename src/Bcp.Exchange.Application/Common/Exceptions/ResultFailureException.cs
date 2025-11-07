using Bcp.Exchange.Core.Shared;

namespace Bcp.Exchange.Application.Common.Exceptions;

public sealed class ResultFailureException : Exception
{
    public Error Error { get; }

    public ResultFailureException(Error error)
        : base(error.Message)
    {
        Error = error;
    }
}
