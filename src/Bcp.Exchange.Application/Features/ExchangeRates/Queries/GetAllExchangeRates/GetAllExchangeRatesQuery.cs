using Bcp.Exchange.Application.Features.ExchangeRates.DTOs;
using Bcp.Exchange.Core.Shared;
using MediatR;

namespace Bcp.Exchange.Application.Features.ExchangeRates.Queries.GetAllExchangeRates;

public sealed class GetAllExchangeRatesQuery : IRequest<Result<IEnumerable<ExchangeRateDto>>> { }
