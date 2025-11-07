using Bcp.Exchange.Application.Features.ExchangeRates.Commands.DeleteExchangeRate;
using Bcp.Exchange.Core.ExchangeRates.Entities;
using Bcp.Exchange.Core.ExchangeRates.Interfaces;
using Bcp.Exchange.Core.Shared.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace Bcp.Exchange.UnitTests.Application.Features.ExchangeRates.Commands;

public class DeleteExchangeRateCommandHandlerTests
{
    private readonly IExchangeRateRepository _exchangeRateRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly DeleteExchangeRateCommandHandler _handler;
    private readonly DeleteExchangeRateCommandValidator _validator;

    public DeleteExchangeRateCommandHandlerTests()
    {
        _exchangeRateRepository = Substitute.For<IExchangeRateRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _unitOfWork.ExchangeRates.Returns(_exchangeRateRepository);

        _handler = new DeleteExchangeRateCommandHandler(_unitOfWork);
        _validator = new DeleteExchangeRateCommandValidator();
    }

    [Fact]
    public async Task Handle_WhenExchangeRateExists_ShouldDeleteExchangeRate()
    {
        var exchangeRateId = Guid.NewGuid();
        var sourceCurrencyId = Guid.NewGuid();
        var targetCurrencyId = Guid.NewGuid();

        var exchangeRate = ExchangeRate.Create(3.75m, sourceCurrencyId, targetCurrencyId, "system");

        var command = new DeleteExchangeRateCommand
        {
            ExchangeRateId = exchangeRateId,
            ModifiedBy = "test-user",
        };

        _exchangeRateRepository
            .GetByIdAsync(exchangeRateId, Arg.Any<CancellationToken>())
            .Returns(exchangeRate);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        exchangeRate.IsActive.Should().BeFalse();
        exchangeRate.ModifiedBy.Should().Be(command.ModifiedBy);
        exchangeRate.ModifiedAt.Should().NotBeNull();

        _exchangeRateRepository.Received(1).Update(exchangeRate);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnValidationErrors()
    {
        var command = new DeleteExchangeRateCommand
        {
            ExchangeRateId = Guid.Empty,
            ModifiedBy = "",
        };

        var validationResult = await _validator.ValidateAsync(command);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().NotBeEmpty();
        validationResult.Errors.Should().Contain(e => e.PropertyName == "ExchangeRateId");
        validationResult.Errors.Should().Contain(e => e.PropertyName == "ModifiedBy");
    }
}
