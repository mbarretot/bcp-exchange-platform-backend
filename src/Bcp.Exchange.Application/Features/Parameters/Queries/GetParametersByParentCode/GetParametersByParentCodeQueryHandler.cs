using Bcp.Exchange.Application.Features.Parameters.DTOs;
using Bcp.Exchange.Core.Configuration;
using Bcp.Exchange.Core.Shared;
using Bcp.Exchange.Core.Shared.Interfaces;
using Mapster;
using MediatR;

namespace Bcp.Exchange.Application.Features.Parameters.Queries.GetParametersByParentCode;

public sealed class GetParametersByParentCodeQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetParametersByParentCodeQuery, Result<IEnumerable<ParameterDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<IEnumerable<ParameterDto>>> Handle(
        GetParametersByParentCodeQuery request,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(request.ParentCode))
        {
            return Result.Failure<IEnumerable<ParameterDto>>(
                ParameterErrors.InvalidCode(request.ParentCode)
            );
        }

        var parent = await _unitOfWork.Parameters.GetByCodeAsync(
            request.ParentCode,
            cancellationToken
        );

        if (parent is null)
        {
            return Result.Failure<IEnumerable<ParameterDto>>(
                ParameterErrors.NotFoundByCode(request.ParentCode)
            );
        }

        var children = await _unitOfWork.Parameters.GetActiveChildrenByParentIdAsync(
            parent.Id,
            cancellationToken
        );

        return Result.Success(children.Adapt<IEnumerable<ParameterDto>>());
    }
}
