using Bcp.Exchange.Application.Features.Parameters.Commands.DeleteParameter;
using Bcp.Exchange.Core.Configuration.Entities;
using Bcp.Exchange.Core.Configuration.Interfaces;
using Bcp.Exchange.Core.Shared.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace Bcp.Exchange.UnitTests.Application.Parameters.Commands;

public class DeleteParameterCommandHandlerTests
{
    private readonly IParameterRepository _parameterRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly DeleteParameterCommandHandler _handler;
    private readonly DeleteParameterCommandValidator _validator;

    public DeleteParameterCommandHandlerTests()
    {
        _parameterRepository = Substitute.For<IParameterRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _unitOfWork.Parameters.Returns(_parameterRepository);

        _handler = new DeleteParameterCommandHandler(_unitOfWork);
        _validator = new DeleteParameterCommandValidator();
    }

    [Fact]
    public async Task Handle_WhenParameterExists_ShouldDeleteParameter()
    {
        var parameterId = Guid.NewGuid();
        var parameter = Parameter.Create("USD", "US Dollar", null, null, 1, null, null, "system");

        var command = new DeleteParameterCommand
        {
            ParameterId = parameterId,
            ModifiedBy = "test-user",
        };

        _parameterRepository
            .GetByIdAsync(parameterId, Arg.Any<CancellationToken>())
            .Returns(parameter);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        parameter.IsActive.Should().BeFalse();
        parameter.ModifiedBy.Should().Be(command.ModifiedBy);
        parameter.ModifiedAt.Should().NotBeNull();

        _parameterRepository.Received(1).Update(parameter);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnValidationErrors()
    {
        var command = new DeleteParameterCommand { ParameterId = Guid.Empty, ModifiedBy = "" };

        var validationResult = await _validator.ValidateAsync(command);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().NotBeEmpty();
        validationResult.Errors.Should().Contain(e => e.PropertyName == "ParameterId");
        validationResult.Errors.Should().Contain(e => e.PropertyName == "ModifiedBy");
    }
}
