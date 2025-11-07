using Bcp.Exchange.Application.Features.ExchangeRates.DTOs;
using Bcp.Exchange.Core.Shared;
using MediatR;

namespace Bcp.Exchange.Application.Features.ExchangeRates.Queries.GetExchangeRateById;

public sealed class GetExchangeRateByIdQuery : IRequest<Result<ExchangeRateDto>>
{
    public Guid Id { get; set; }
}
