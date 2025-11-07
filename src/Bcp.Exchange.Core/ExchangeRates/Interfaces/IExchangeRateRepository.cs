using Bcp.Exchange.Core.ExchangeRates.Entities;
using Bcp.Exchange.Core.Shared.Interfaces;

namespace Bcp.Exchange.Core.ExchangeRates.Interfaces;

public interface IExchangeRateRepository : IRepository<ExchangeRate>
{
    Task<IEnumerable<ExchangeRate>> GetAllActiveAsync(
        CancellationToken cancellationToken = default
    );

    Task<ExchangeRate?> GetByCurrencyPairIncludingInactiveAsync(
        Guid sourceCurrencyId,
        Guid targetCurrencyId,
        CancellationToken cancellationToken = default
    );

    Task<bool> ExistsActiveByCurrencyPairAsync(
        Guid sourceCurrencyId,
        Guid targetCurrencyId,
        CancellationToken cancellationToken = default
    );
}
