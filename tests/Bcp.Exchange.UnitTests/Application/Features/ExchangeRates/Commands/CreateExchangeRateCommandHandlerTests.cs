using Bcp.Exchange.Application.Features.ExchangeRates.Commands.CreateExchangeRate;
using Bcp.Exchange.Core.Configuration.Entities;
using Bcp.Exchange.Core.Configuration.Interfaces;
using Bcp.Exchange.Core.ExchangeRates.Entities;
using Bcp.Exchange.Core.ExchangeRates.Interfaces;
using Bcp.Exchange.Core.Shared.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace Bcp.Exchange.UnitTests.Application.Features.ExchangeRates.Commands;

public class CreateExchangeRateCommandHandlerTests
{
    private readonly IExchangeRateRepository _exchangeRateRepository;
    private readonly IParameterRepository _parameterRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateExchangeRateCommandHandler _handler;
    private readonly CreateExchangeRateCommandValidator _validator;

    public CreateExchangeRateCommandHandlerTests()
    {
        _exchangeRateRepository = Substitute.For<IExchangeRateRepository>();
        _parameterRepository = Substitute.For<IParameterRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _unitOfWork.ExchangeRates.Returns(_exchangeRateRepository);
        _unitOfWork.Parameters.Returns(_parameterRepository);

        _handler = new CreateExchangeRateCommandHandler(_unitOfWork);
        _validator = new CreateExchangeRateCommandValidator();
    }

    [Fact]
    public async Task Handle_WhenCurrencyPairIsUnique_ShouldCreateExchangeRate()
    {
        var sourceCurrencyId = Guid.NewGuid();
        var targetCurrencyId = Guid.NewGuid();

        var command = new CreateExchangeRateCommand
        {
            Rate = 3.75m,
            CurrencySourceId = sourceCurrencyId,
            CurrencyTargetId = targetCurrencyId,
            CreatedBy = "test-user",
        };

        var sourceCurrency = Parameter.Create(
            "USD",
            "US Dollar",
            null,
            null,
            1,
            null,
            null,
            "system"
        );
        var targetCurrency = Parameter.Create(
            "PEN",
            "Peruvian Sol",
            null,
            null,
            2,
            null,
            null,
            "system"
        );

        _parameterRepository
            .GetByIdAsync(sourceCurrencyId, Arg.Any<CancellationToken>())
            .Returns(sourceCurrency);

        _parameterRepository
            .GetByIdAsync(targetCurrencyId, Arg.Any<CancellationToken>())
            .Returns(targetCurrency);

        _exchangeRateRepository
            .ExistsActiveByCurrencyPairAsync(
                sourceCurrencyId,
                targetCurrencyId,
                Arg.Any<CancellationToken>()
            )
            .Returns(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Rate.Should().Be(command.Rate);
        result.Value.CurrencySourceId.Should().Be(command.CurrencySourceId);
        result.Value.CurrencyTargetId.Should().Be(command.CurrencyTargetId);

        await _exchangeRateRepository
            .Received(1)
            .AddAsync(Arg.Any<ExchangeRate>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnValidationErrors()
    {
        var command = new CreateExchangeRateCommand
        {
            Rate = -1,
            CurrencySourceId = Guid.Empty,
            CurrencyTargetId = Guid.Empty,
            CreatedBy = "",
        };

        var validationResult = await _validator.ValidateAsync(command);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().NotBeEmpty();
        validationResult.Errors.Should().Contain(e => e.PropertyName == "Rate");
        validationResult.Errors.Should().Contain(e => e.PropertyName == "CurrencySourceId");
        validationResult.Errors.Should().Contain(e => e.PropertyName == "CurrencyTargetId");
        validationResult.Errors.Should().Contain(e => e.PropertyName == "CreatedBy");
    }
}
