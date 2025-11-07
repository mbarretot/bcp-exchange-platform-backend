using Bcp.Exchange.Core.Configuration.Entities;
using Bcp.Exchange.Core.Configuration.Interfaces;
using Bcp.Exchange.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Bcp.Exchange.Infrastructure.Repositories;

public class ParameterRepository(ExchangeDbContext context) : IParameterRepository
{
    private readonly ExchangeDbContext _context = context;

    public async Task<Parameter?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        return await _context.Parameters.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Parameter>> GetAllAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await _context.Parameters.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Parameter entity, CancellationToken cancellationToken = default)
    {
        await _context.Parameters.AddAsync(entity, cancellationToken);
    }

    public void Update(Parameter entity)
    {
        _context.Parameters.Update(entity);
    }

    public async Task<Parameter?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default
    )
    {
        return await _context.Parameters.FirstOrDefaultAsync(
            p => p.Code == code,
            cancellationToken
        );
    }

    public async Task<Parameter?> GetByCodeIncludingInactiveAsync(
        string code,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .Parameters.IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Code == code, cancellationToken);
    }

    public async Task<IEnumerable<Parameter>> GetAllActiveAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await _context.Parameters.Where(p => p.IsActive).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Parameter>> GetActiveChildrenByParentCodeAsync(
        string parentCode,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .Parameters.Where(p => p.Parent!.Code == parentCode && p.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Parameter>> GetActiveChildrenByParentIdAsync(
        Guid parentId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .Parameters.Where(p => p.ParentId == parentId && p.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default
    )
    {
        return await _context.Parameters.AnyAsync(p => p.Code == code, cancellationToken);
    }
}
