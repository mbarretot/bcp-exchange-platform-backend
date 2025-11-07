using Bcp.Exchange.Core.Shared;
using MediatR;

namespace Bcp.Exchange.Application.Features.Parameters.Commands.DeleteParameter;

public sealed class DeleteParameterCommand : IRequest<Result>
{
    public Guid ParameterId { get; set; }
    public string? ModifiedBy { get; set; }
}
