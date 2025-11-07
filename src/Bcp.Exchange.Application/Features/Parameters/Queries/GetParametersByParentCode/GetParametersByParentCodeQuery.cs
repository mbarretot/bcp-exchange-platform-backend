using Bcp.Exchange.Application.Features.Parameters.DTOs;
using Bcp.Exchange.Core.Shared;
using MediatR;

namespace Bcp.Exchange.Application.Features.Parameters.Queries.GetParametersByParentCode;

public sealed class GetParametersByParentCodeQuery : IRequest<Result<IEnumerable<ParameterDto>>>
{
    public string ParentCode { get; set; } = string.Empty;
}
