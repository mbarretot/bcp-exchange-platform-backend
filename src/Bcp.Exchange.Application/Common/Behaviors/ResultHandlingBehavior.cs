using Bcp.Exchange.Application.Common.Exceptions;
using Bcp.Exchange.Core.Shared;
using MediatR;

namespace Bcp.Exchange.Application.Common.Behaviors;

public sealed class ResultHandlingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var response = await next();

        if (response is Result result && result.IsFailure)
        {
            throw new ResultFailureException(result.Error);
        }

        return response;
    }
}
