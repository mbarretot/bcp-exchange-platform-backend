using Bcp.Exchange.Application.Features.ExchangeRates.DTOs;
using Bcp.Exchange.Core.Shared;
using Bcp.Exchange.Core.Shared.Interfaces;
using Mapster;
using MediatR;

namespace Bcp.Exchange.Application.Features.ExchangeRates.Queries.GetAllExchangeRates;

public sealed class GetAllExchangeRatesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAllExchangeRatesQuery, Result<IEnumerable<ExchangeRateDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<IEnumerable<ExchangeRateDto>>> Handle(
        GetAllExchangeRatesQuery request,
        CancellationToken cancellationToken
    )
    {
        var exchangeRates = await _unitOfWork.ExchangeRates.GetAllActiveAsync(cancellationToken);

        return Result.Success(exchangeRates.Adapt<IEnumerable<ExchangeRateDto>>());
    }
}
