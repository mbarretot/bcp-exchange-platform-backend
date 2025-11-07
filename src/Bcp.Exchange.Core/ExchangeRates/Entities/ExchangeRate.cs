using Bcp.Exchange.Core.Configuration.Entities;

namespace Bcp.Exchange.Core.ExchangeRates.Entities;

public sealed class ExchangeRate
{
    public Guid Id { get; private set; }
    public decimal Rate { get; private set; }
    public Guid CurrencySourceId { get; private set; }
    public Guid CurrencyTargetId { get; private set; }

    public Parameter? CurrencySource { get; private set; }
    public Parameter? CurrencyTarget { get; private set; }

    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public string? CreatedBy { get; private set; }
    public string? ModifiedBy { get; private set; }

    private ExchangeRate() { }

    public static ExchangeRate Create(
        decimal rate,
        Guid currencySourceId,
        Guid currencyTargetId,
        string? createdBy
    )
    {
        return new ExchangeRate
        {
            Id = Guid.NewGuid(),
            Rate = rate,
            CurrencySourceId = currencySourceId,
            CurrencyTargetId = currencyTargetId,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        };
    }

    public void Update(
        decimal? rate,
        Guid? currencySourceId,
        Guid? currencyTargetId,
        string? modifiedBy
    )
    {
        if (rate.HasValue)
            Rate = rate.Value;

        if (currencySourceId.HasValue)
            CurrencySourceId = currencySourceId.Value;

        if (currencyTargetId.HasValue)
            CurrencyTargetId = currencyTargetId.Value;

        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public void Delete(string? modifiedBy)
    {
        IsActive = false;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public void Reactivate(
        decimal rate,
        Guid currencySourceId,
        Guid currencyTargetId,
        string? modifiedBy
    )
    {
        Rate = rate;
        CurrencySourceId = currencySourceId;
        CurrencyTargetId = currencyTargetId;
        IsActive = true;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }
}
