using Bcp.Exchange.Application.Features.ExchangeRates.DTOs;
using Bcp.Exchange.Core.Shared;
using MediatR;

namespace Bcp.Exchange.Application.Features.ExchangeRates.Commands.CreateExchangeRate;

public class CreateExchangeRateCommand : IRequest<Result<ExchangeRateDto>>
{
    public decimal Rate { get; set; }
    public Guid CurrencySourceId { get; set; }
    public Guid CurrencyTargetId { get; set; }
    public string? CreatedBy { get; set; }
}
