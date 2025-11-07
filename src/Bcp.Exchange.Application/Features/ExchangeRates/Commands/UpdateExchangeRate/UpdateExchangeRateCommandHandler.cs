using Bcp.Exchange.Application.Features.ExchangeRates.DTOs;
using Bcp.Exchange.Core.ExchangeRates;
using Bcp.Exchange.Core.Shared;
using Bcp.Exchange.Core.Shared.Interfaces;
using Mapster;
using MediatR;

namespace Bcp.Exchange.Application.Features.ExchangeRates.Commands.UpdateExchangeRate;

public class UpdateExchangeRateCommandHandler
    : IRequestHandler<UpdateExchangeRateCommand, Result<ExchangeRateDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateExchangeRateCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ExchangeRateDto>> Handle(
        UpdateExchangeRateCommand request,
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

        if (request.Rate.HasValue && request.Rate.Value <= 0)
            return Result.Failure<ExchangeRateDto>(ExchangeRateErrors.InvalidRate());

        if (request.CurrencySourceId.HasValue)
        {
            var sourceCurrency = await _unitOfWork.Parameters.GetByIdAsync(
                request.CurrencySourceId.Value,
                cancellationToken
            );
            if (sourceCurrency == null)
                return Result.Failure<ExchangeRateDto>(
                    ExchangeRateErrors.CurrencyNotFound(request.CurrencySourceId.Value)
                );
        }

        if (request.CurrencyTargetId.HasValue)
        {
            var targetCurrency = await _unitOfWork.Parameters.GetByIdAsync(
                request.CurrencyTargetId.Value,
                cancellationToken
            );
            if (targetCurrency == null)
                return Result.Failure<ExchangeRateDto>(
                    ExchangeRateErrors.CurrencyNotFound(request.CurrencyTargetId.Value)
                );
        }

        exchangeRate.Update(
            request.Rate,
            request.CurrencySourceId,
            request.CurrencyTargetId,
            request.ModifiedBy
        );

        _unitOfWork.ExchangeRates.Update(exchangeRate);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ExchangeRateDto>.Success(exchangeRate.Adapt<ExchangeRateDto>());
    }
}
