using Bcp.Exchange.Core.Configuration.Interfaces;
using Bcp.Exchange.Core.ExchangeRates.Interfaces;
using Bcp.Exchange.Core.Shared.Interfaces;
using Bcp.Exchange.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace Bcp.Exchange.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ExchangeDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(
        ExchangeDbContext context,
        IParameterRepository parameters,
        IExchangeRateRepository exchangeRates
    )
    {
        _context = context;
        Parameters = parameters;
        ExchangeRates = exchangeRates;
    }

    public IParameterRepository Parameters { get; }
    public IExchangeRateRepository ExchangeRates { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
