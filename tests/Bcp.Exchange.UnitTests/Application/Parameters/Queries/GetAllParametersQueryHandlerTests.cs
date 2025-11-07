using Bcp.Exchange.Application.Features.Parameters.Queries.GetAllParameters;
using Bcp.Exchange.Core.Configuration.Entities;
using Bcp.Exchange.Core.Configuration.Interfaces;
using Bcp.Exchange.Core.Shared.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace Bcp.Exchange.UnitTests.Application.Parameters.Queries;

public class GetAllParametersQueryHandlerTests
{
    private readonly IParameterRepository _parameterRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly GetAllParametersQueryHandler _handler;

    public GetAllParametersQueryHandlerTests()
    {
        _parameterRepository = Substitute.For<IParameterRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _unitOfWork.Parameters.Returns(_parameterRepository);

        _handler = new GetAllParametersQueryHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenParametersExist_ShouldReturnAllActiveParameters()
    {
        var parameters = new List<Parameter>
        {
            Parameter.Create("CURRENCY", "Currency", null, null, 1, null, null, "system"),
            Parameter.Create("USD", "US Dollar", null, null, 1, null, null, "system"),
            Parameter.Create("EUR", "Euro", null, null, 2, null, null, "system"),
        };

        var query = new GetAllParametersQuery();

        _parameterRepository.GetAllActiveAsync(Arg.Any<CancellationToken>()).Returns(parameters);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(3);
        result.Value.Should().Contain(p => p.Code == "CURRENCY");
        result.Value.Should().Contain(p => p.Code == "USD");
        result.Value.Should().Contain(p => p.Code == "EUR");
    }
}
