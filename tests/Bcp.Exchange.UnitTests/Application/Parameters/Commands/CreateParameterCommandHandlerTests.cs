using Bcp.Exchange.Application.Features.Parameters.Commands.CreateParameter;
using Bcp.Exchange.Core.Configuration.Entities;
using Bcp.Exchange.Core.Configuration.Interfaces;
using Bcp.Exchange.Core.Shared.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace Bcp.Exchange.UnitTests.Application.Parameters.Commands;

public class CreateParameterCommandHandlerTests
{
    private readonly IParameterRepository _parameterRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateParameterCommandHandler _handler;
    private readonly CreateParameterCommandValidator _validator;

    public CreateParameterCommandHandlerTests()
    {
        _parameterRepository = Substitute.For<IParameterRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _unitOfWork.Parameters.Returns(_parameterRepository);

        _handler = new CreateParameterCommandHandler(_unitOfWork);
        _validator = new CreateParameterCommandValidator();
    }

    [Fact]
    public async Task Handle_WhenCodeIsUnique_ShouldCreateParameter()
    {
        var command = new CreateParameterCommand
        {
            Code = "USD",
            Description = "US Dollar",
            LongDescription = "United States Dollar",
            ParentId = null,
            DisplayOrder = 1,
            NumericValue = null,
            TextValue = null,
            CreatedBy = "test-user",
        };

        _parameterRepository
            .ExistsByCodeAsync(command.Code, Arg.Any<CancellationToken>())
            .Returns(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Code.Should().Be(command.Code);
        result.Value.Description.Should().Be(command.Description);

        await _parameterRepository
            .Received(1)
            .ExistsByCodeAsync(command.Code, Arg.Any<CancellationToken>());
        await _parameterRepository
            .Received(1)
            .AddAsync(Arg.Any<Parameter>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnValidationErrors()
    {
        var command = new CreateParameterCommand
        {
            Code = "",
            Description = "",
            LongDescription = null,
            ParentId = null,
            DisplayOrder = -1,
            NumericValue = null,
            TextValue = null,
            CreatedBy = "",
        };

        var validationResult = await _validator.ValidateAsync(command);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().NotBeEmpty();
        validationResult.Errors.Should().Contain(e => e.PropertyName == "Code");
        validationResult.Errors.Should().Contain(e => e.PropertyName == "Description");
        validationResult.Errors.Should().Contain(e => e.PropertyName == "DisplayOrder");
        validationResult.Errors.Should().Contain(e => e.PropertyName == "CreatedBy");
    }
}
