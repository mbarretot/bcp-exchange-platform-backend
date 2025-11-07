using Bcp.Exchange.Application.Features.Parameters.Commands.UpdateParameter;
using Bcp.Exchange.Core.Configuration.Entities;
using Bcp.Exchange.Core.Configuration.Interfaces;
using Bcp.Exchange.Core.Shared.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace Bcp.Exchange.UnitTests.Application.Parameters.Commands;

public class UpdateParameterCommandHandlerTests
{
    private readonly IParameterRepository _parameterRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UpdateParameterCommandHandler _handler;
    private readonly UpdateParameterCommandValidator _validator;

    public UpdateParameterCommandHandlerTests()
    {
        _parameterRepository = Substitute.For<IParameterRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _unitOfWork.Parameters.Returns(_parameterRepository);

        _handler = new UpdateParameterCommandHandler(_unitOfWork);
        _validator = new UpdateParameterCommandValidator();
    }

    [Fact]
    public async Task Handle_WhenParameterExists_ShouldUpdateParameter()
    {
        var parameterId = Guid.NewGuid();
        var existingParameter = Parameter.Create(
            "USD",
            "US Dollar",
            "United States Dollar",
            null,
            1,
            null,
            null,
            "system"
        );

        var command = new UpdateParameterCommand
        {
            ParameterId = parameterId,
            Description = "Updated US Dollar",
            LongDescription = "Updated Description",
            DisplayOrder = 2,
            NumericValue = 1.5m,
            TextValue = "test",
            ModifiedBy = "test-user",
        };

        _parameterRepository
            .GetByIdAsync(parameterId, Arg.Any<CancellationToken>())
            .Returns(existingParameter);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Description.Should().Be(command.Description);
        result.Value.LongDescription.Should().Be(command.LongDescription);
        result.Value.DisplayOrder.Should().Be(command.DisplayOrder);
        result.Value.NumericValue.Should().Be(command.NumericValue);
        result.Value.TextValue.Should().Be(command.TextValue);
        result.Value.ModifiedBy.Should().Be(command.ModifiedBy);
        result.Value.ModifiedAt.Should().NotBeNull();

        _parameterRepository.Received(1).Update(existingParameter);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnValidationErrors()
    {
        var command = new UpdateParameterCommand
        {
            ParameterId = Guid.Empty,
            Description = new string('a', 201),
            LongDescription = new string('b', 501),
            DisplayOrder = -1,
            NumericValue = null,
            TextValue = null,
            ModifiedBy = "",
        };

        var validationResult = await _validator.ValidateAsync(command);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().NotBeEmpty();
        validationResult.Errors.Should().Contain(e => e.PropertyName == "ParameterId");
        validationResult.Errors.Should().Contain(e => e.PropertyName == "Description");
        validationResult.Errors.Should().Contain(e => e.PropertyName == "LongDescription");
        validationResult.Errors.Should().Contain(e => e.PropertyName == "DisplayOrder");
        validationResult.Errors.Should().Contain(e => e.PropertyName == "ModifiedBy");
    }
}
