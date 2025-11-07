using Bcp.Exchange.Application.Features.Parameters.Queries.GetParametersByParentCode;
using Bcp.Exchange.Core.Configuration.Entities;
using Bcp.Exchange.Core.Configuration.Interfaces;
using Bcp.Exchange.Core.Shared.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace Bcp.Exchange.UnitTests.Application.Parameters.Queries;

public class GetParametersByParentCodeQueryHandlerTests
{
    private readonly IParameterRepository _parameterRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly GetParametersByParentCodeQueryHandler _handler;

    public GetParametersByParentCodeQueryHandlerTests()
    {
        _parameterRepository = Substitute.For<IParameterRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _unitOfWork.Parameters.Returns(_parameterRepository);

        _handler = new GetParametersByParentCodeQueryHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenParentExistsWithChildren_ShouldReturnChildren()
    {
        var parentCode = "CURRENCY";
        var parent = Parameter.Create(parentCode, "Currency", null, null, 1, null, null, "system");

        var children = new List<Parameter>
        {
            Parameter.Create("USD", "US Dollar", null, parent.Id, 1, null, null, "system"),
            Parameter.Create("EUR", "Euro", null, parent.Id, 2, null, null, "system"),
            Parameter.Create("PEN", "Peruvian Sol", null, parent.Id, 3, null, null, "system"),
        };

        var query = new GetParametersByParentCodeQuery { ParentCode = parentCode };

        _parameterRepository
            .GetByCodeAsync(parentCode, Arg.Any<CancellationToken>())
            .Returns(parent);

        _parameterRepository
            .GetActiveChildrenByParentIdAsync(parent.Id, Arg.Any<CancellationToken>())
            .Returns(children);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        result.Value.Should().Contain(p => p.Code == "USD");
        result.Value.Should().Contain(p => p.Code == "EUR");
        result.Value.Should().Contain(p => p.Code == "PEN");
    }
}
