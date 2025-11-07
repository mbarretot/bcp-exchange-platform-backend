using Bcp.Exchange.Application.Features.Parameters.DTOs;
using Bcp.Exchange.Core.Shared;
using MediatR;

namespace Bcp.Exchange.Application.Features.Parameters.Commands.CreateParameter;

public sealed class CreateParameterCommand : IRequest<Result<ParameterDto>>
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? LongDescription { get; set; }
    public Guid? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public decimal? NumericValue { get; set; }
    public string? TextValue { get; set; }
    public string? CreatedBy { get; set; }
}
