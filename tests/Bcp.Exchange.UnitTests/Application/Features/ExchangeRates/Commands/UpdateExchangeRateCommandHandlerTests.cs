using Bcp.Exchange.Application.Features.ExchangeRates.Commands.UpdateExchangeRate;
using Bcp.Exchange.Core.Configuration.Interfaces;
using Bcp.Exchange.Core.ExchangeRates.Entities;
using Bcp.Exchange.Core.ExchangeRates.Interfaces;
using Bcp.Exchange.Core.Shared.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace Bcp.Exchange.UnitTests.Application.Features.ExchangeRates.Commands;

public class UpdateExchangeRateCommandHandlerTests
{
    private readonly IExchangeRateRepository _exchangeRateRepository;
    private readonly IParameterRepository _parameterRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UpdateExchangeRateCommandHandler _handler;
    private readonly UpdateExchangeRateCommandValidator _validator;

    public UpdateExchangeRateCommandHandlerTests()
    {
        _exchangeRateRepository = Substitute.For<IExchangeRateRepository>();
        _parameterRepository = Substitute.For<IParameterRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _unitOfWork.ExchangeRates.Returns(_exchangeRateRepository);
        _unitOfWork.Parameters.Returns(_parameterRepository);

        _handler = new UpdateExchangeRateCommandHandler(_unitOfWork);
        _validator = new UpdateExchangeRateCommandValidator();
    }

    [Fact]
    public async Task Handle_WhenExchangeRateExists_ShouldUpdateExchangeRate()
    {
        var exchangeRateId = Guid.NewGuid();
        var sourceCurrencyId = Guid.NewGuid();
        var targetCurrencyId = Guid.NewGuid();

        var existingExchangeRate = ExchangeRate.Create(
            3.75m,
            sourceCurrencyId,
            targetCurrencyId,
            "system"
        );

        var command = new UpdateExchangeRateCommand
        {
            ExchangeRateId = exchangeRateId,
            Rate = 3.80m,
            CurrencySourceId = null,
            CurrencyTargetId = null,
            ModifiedBy = "test-user",
        };

        _exchangeRateRepository
            .GetByIdAsync(exchangeRateId, Arg.Any<CancellationToken>())
            .Returns(existingExchangeRate);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Rate.Should().Be(command.Rate);
        result.Value.ModifiedBy.Should().Be(command.ModifiedBy);
        result.Value.ModifiedAt.Should().NotBeNull();

        _exchangeRateRepository.Received(1).Update(existingExchangeRate);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnValidationErrors()
    {
        var command = new UpdateExchangeRateCommand
        {
            ExchangeRateId = Guid.Empty,
            Rate = -1,
            CurrencySourceId = null,
            CurrencyTargetId = null,
            ModifiedBy = "",
        };

        var validationResult = await _validator.ValidateAsync(command);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().NotBeEmpty();
        validationResult.Errors.Should().Contain(e => e.PropertyName == "ExchangeRateId");
        validationResult.Errors.Should().Contain(e => e.PropertyName == "Rate");
        validationResult.Errors.Should().Contain(e => e.PropertyName == "ModifiedBy");
    }
}
