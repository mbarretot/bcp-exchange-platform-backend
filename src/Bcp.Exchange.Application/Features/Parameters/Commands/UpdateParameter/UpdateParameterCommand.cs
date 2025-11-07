using Bcp.Exchange.Application.Features.Parameters.DTOs;
using Bcp.Exchange.Core.Shared;
using MediatR;

namespace Bcp.Exchange.Application.Features.Parameters.Commands.UpdateParameter;

public sealed class UpdateParameterCommand : IRequest<Result<ParameterDto>>
{
    public Guid ParameterId { get; set; }
    public string? Description { get; set; }
    public string? LongDescription { get; set; }
    public int? DisplayOrder { get; set; }
    public decimal? NumericValue { get; set; }
    public string? TextValue { get; set; }
    public string? ModifiedBy { get; set; }
}
