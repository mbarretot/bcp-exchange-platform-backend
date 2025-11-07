using Bcp.Exchange.Application.Features.ExchangeRates.DTOs;
using Bcp.Exchange.Core.Shared;
using MediatR;

namespace Bcp.Exchange.Application.Features.ExchangeRates.Commands.UpdateExchangeRate;

public class UpdateExchangeRateCommand : IRequest<Result<ExchangeRateDto>>
{
    public Guid ExchangeRateId { get; set; }
    public decimal? Rate { get; set; }
    public Guid? CurrencySourceId { get; set; }
    public Guid? CurrencyTargetId { get; set; }
    public string? ModifiedBy { get; set; }
}
