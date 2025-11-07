using Bcp.Exchange.Core.Configuration.Interfaces;
using Bcp.Exchange.Core.ExchangeRates.Interfaces;

namespace Bcp.Exchange.Core.Shared.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IExchangeRateRepository ExchangeRates { get; }
    IParameterRepository Parameters { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
