using Bcp.Exchange.Core.Configuration.Entities;
using Bcp.Exchange.Core.Shared.Interfaces;

namespace Bcp.Exchange.Core.Configuration.Interfaces;

public interface IParameterRepository : IRepository<Parameter>
{
    Task<Parameter?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<Parameter?> GetByCodeIncludingInactiveAsync(
        string code,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<Parameter>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<Parameter>> GetActiveChildrenByParentCodeAsync(
        string parentCode,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<Parameter>> GetActiveChildrenByParentIdAsync(
        Guid parentId,
        CancellationToken cancellationToken = default
    );

    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
}
