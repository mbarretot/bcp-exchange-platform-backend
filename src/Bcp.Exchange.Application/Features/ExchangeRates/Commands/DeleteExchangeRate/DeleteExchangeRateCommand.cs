using Bcp.Exchange.Application.Features.ExchangeRates.DTOs;
using Bcp.Exchange.Core.Shared;
using MediatR;

namespace Bcp.Exchange.Application.Features.ExchangeRates.Commands.DeleteExchangeRate;

public class DeleteExchangeRateCommand : IRequest<Result<ExchangeRateDto>>
{
    public Guid ExchangeRateId { get; set; }
    public string? ModifiedBy { get; set; }
}
