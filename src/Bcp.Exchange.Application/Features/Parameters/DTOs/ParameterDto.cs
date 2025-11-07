namespace Bcp.Exchange.Application.Features.Parameters.DTOs;

public sealed record ParameterDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? LongDescription { get; init; }
    public Guid? ParentId { get; init; }
    public int DisplayOrder { get; init; }
    public decimal? NumericValue { get; init; }
    public string? TextValue { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
    public string? CreatedBy { get; init; }
    public string? ModifiedBy { get; init; }
}
