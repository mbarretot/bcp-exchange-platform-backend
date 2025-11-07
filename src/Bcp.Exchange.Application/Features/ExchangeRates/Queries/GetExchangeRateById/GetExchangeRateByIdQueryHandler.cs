using Bcp.Exchange.Application.Features.ExchangeRates.DTOs;
using Bcp.Exchange.Core.ExchangeRates;
using Bcp.Exchange.Core.Shared;
using Bcp.Exchange.Core.Shared.Interfaces;
using Mapster;
using MediatR;

namespace Bcp.Exchange.Application.Features.ExchangeRates.Queries.GetExchangeRateById;

public sealed class GetExchangeRateByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetExchangeRateByIdQuery, Result<ExchangeRateDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<ExchangeRateDto>> Handle(
        GetExchangeRateByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var exchangeRate = await _unitOfWork.ExchangeRates.GetByIdAsync(
            request.Id,
            cancellationToken
        );

        if (exchangeRate is null)
        {
            return Result.Failure<ExchangeRateDto>(ExchangeRateErrors.NotFound(request.Id));
        }

        return Result.Success(exchangeRate.Adapt<ExchangeRateDto>());
    }
}
