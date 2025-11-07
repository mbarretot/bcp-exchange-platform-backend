using Bcp.Exchange.Application.Features.ExchangeRates.DTOs;
using Bcp.Exchange.Core.ExchangeRates;
using Bcp.Exchange.Core.Shared;
using Bcp.Exchange.Core.Shared.Interfaces;
using Mapster;
using MediatR;

namespace Bcp.Exchange.Application.Features.ExchangeRates.Commands.DeleteExchangeRate;

public class DeleteExchangeRateCommandHandler
    : IRequestHandler<DeleteExchangeRateCommand, Result<ExchangeRateDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteExchangeRateCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ExchangeRateDto>> Handle(
        DeleteExchangeRateCommand request,
        CancellationToken cancellationToken
    )
    {
        var exchangeRate = await _unitOfWork.ExchangeRates.GetByIdAsync(
            request.ExchangeRateId,
            cancellationToken
        );
        if (exchangeRate == null)
            return Result.Failure<ExchangeRateDto>(
                ExchangeRateErrors.NotFound(request.ExchangeRateId)
            );

        exchangeRate.Delete(request.ModifiedBy);

        _unitOfWork.ExchangeRates.Update(exchangeRate);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(exchangeRate.Adapt<ExchangeRateDto>());
    }
}
