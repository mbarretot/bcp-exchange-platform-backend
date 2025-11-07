using Bcp.Exchange.Application.Features.Parameters.DTOs;
using Bcp.Exchange.Core.Shared;
using Bcp.Exchange.Core.Shared.Interfaces;
using Mapster;
using MediatR;

namespace Bcp.Exchange.Application.Features.Parameters.Queries.GetAllParameters;

public sealed class GetAllParametersQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAllParametersQuery, Result<IEnumerable<ParameterDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<IEnumerable<ParameterDto>>> Handle(
        GetAllParametersQuery request,
        CancellationToken cancellationToken
    )
    {
        var parameters = await _unitOfWork.Parameters.GetAllActiveAsync(cancellationToken);

        return Result.Success(parameters.Adapt<IEnumerable<ParameterDto>>());
    }
}
