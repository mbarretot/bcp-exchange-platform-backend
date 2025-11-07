using Bcp.Exchange.Core.ExchangeRates.Entities;
using Bcp.Exchange.Core.ExchangeRates.Interfaces;
using Bcp.Exchange.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Bcp.Exchange.Infrastructure.Repositories;

public class ExchangeRateRepository : IExchangeRateRepository
{
    private readonly ExchangeDbContext _context;

    public ExchangeRateRepository(ExchangeDbContext context)
    {
        _context = context;
    }

    public async Task<ExchangeRate?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        return await _context.ExchangeRates.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ExchangeRate>> GetAllAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await _context.ExchangeRates.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ExchangeRate entity, CancellationToken cancellationToken = default)
    {
        await _context.ExchangeRates.AddAsync(entity, cancellationToken);
    }

    public void Update(ExchangeRate entity)
    {
        _context.ExchangeRates.Update(entity);
    }

    public async Task<IEnumerable<ExchangeRate>> GetAllActiveAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await _context.ExchangeRates.Where(e => e.IsActive).ToListAsync(cancellationToken);
    }

    public async Task<ExchangeRate?> GetByCurrencyPairIncludingInactiveAsync(
        Guid sourceCurrencyId,
        Guid targetCurrencyId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .ExchangeRates.IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                e =>
                    e.CurrencySourceId == sourceCurrencyId
                    && e.CurrencyTargetId == targetCurrencyId,
                cancellationToken
            );
    }

    public async Task<bool> ExistsActiveByCurrencyPairAsync(
        Guid sourceCurrencyId,
        Guid targetCurrencyId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context.ExchangeRates.AnyAsync(
            e =>
                e.CurrencySourceId == sourceCurrencyId
                && e.CurrencyTargetId == targetCurrencyId
                && e.IsActive,
            cancellationToken
        );
    }
}
