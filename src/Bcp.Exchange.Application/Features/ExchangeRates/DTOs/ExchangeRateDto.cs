namespace Bcp.Exchange.Application.Features.ExchangeRates.DTOs;

public record ExchangeRateDto
{
    public Guid Id { get; init; }
    public decimal Rate { get; init; }
    public Guid CurrencySourceId { get; init; }
    public Guid CurrencyTargetId { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
    public string? CreatedBy { get; init; }
    public string? ModifiedBy { get; init; }
}
