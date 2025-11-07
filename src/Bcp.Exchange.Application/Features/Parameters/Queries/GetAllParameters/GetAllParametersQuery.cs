using Bcp.Exchange.Application.Features.Parameters.DTOs;
using Bcp.Exchange.Core.Shared;
using MediatR;

namespace Bcp.Exchange.Application.Features.Parameters.Queries.GetAllParameters;

public sealed class GetAllParametersQuery : IRequest<Result<IEnumerable<ParameterDto>>> { }
