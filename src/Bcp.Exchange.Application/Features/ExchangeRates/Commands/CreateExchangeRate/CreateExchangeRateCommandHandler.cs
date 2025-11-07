using Bcp.Exchange.Application.Features.ExchangeRates.DTOs;
using Bcp.Exchange.Core.ExchangeRates;
using Bcp.Exchange.Core.ExchangeRates.Entities;
using Bcp.Exchange.Core.Shared;
using Bcp.Exchange.Core.Shared.Interfaces;
using Mapster;
using MediatR;

namespace Bcp.Exchange.Application.Features.ExchangeRates.Commands.CreateExchangeRate;

public class CreateExchangeRateCommandHandler
    : IRequestHandler<CreateExchangeRateCommand, Result<ExchangeRateDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateExchangeRateCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ExchangeRateDto>> Handle(
        CreateExchangeRateCommand request,
        CancellationToken cancellationToken
    )
    {
        if (request.Rate <= 0)
            return Result.Failure<ExchangeRateDto>(ExchangeRateErrors.InvalidRate());

        if (request.CurrencySourceId == request.CurrencyTargetId)
            return Result.Failure<ExchangeRateDto>(ExchangeRateErrors.SameCurrency());

        var sourceCurrency = await _unitOfWork.Parameters.GetByIdAsync(
            request.CurrencySourceId,
            cancellationToken
        );
        if (sourceCurrency == null)
            return Result.Failure<ExchangeRateDto>(
                ExchangeRateErrors.CurrencyNotFound(request.CurrencySourceId)
            );

        var targetCurrency = await _unitOfWork.Parameters.GetByIdAsync(
            request.CurrencyTargetId,
            cancellationToken
        );
        if (targetCurrency == null)
            return Result.Failure<ExchangeRateDto>(
                ExchangeRateErrors.CurrencyNotFound(request.CurrencyTargetId)
            );

        var existingExchangeRate =
            await _unitOfWork.ExchangeRates.GetByCurrencyPairIncludingInactiveAsync(
                request.CurrencySourceId,
                request.CurrencyTargetId,
                cancellationToken
            );

        if (existingExchangeRate is not null)
        {
            if (existingExchangeRate.IsActive)
            {
                return Result.Failure<ExchangeRateDto>(ExchangeRateErrors.AlreadyExists());
            }

            existingExchangeRate.Reactivate(
                request.Rate,
                request.CurrencySourceId,
                request.CurrencyTargetId,
                request.CreatedBy
            );

            _unitOfWork.ExchangeRates.Update(existingExchangeRate);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(existingExchangeRate.Adapt<ExchangeRateDto>());
        }

        var exchangeRate = ExchangeRate.Create(
            request.Rate,
            request.CurrencySourceId,
            request.CurrencyTargetId,
            request.CreatedBy
        );

        await _unitOfWork.ExchangeRates.AddAsync(exchangeRate, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(exchangeRate.Adapt<ExchangeRateDto>());
    }
}
