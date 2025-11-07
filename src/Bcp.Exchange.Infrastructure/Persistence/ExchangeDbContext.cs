using Bcp.Exchange.Core.Configuration.Entities;
using Bcp.Exchange.Core.ExchangeRates.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bcp.Exchange.Infrastructure.Persistence;

public class ExchangeDbContext : DbContext
{
    public ExchangeDbContext(DbContextOptions<ExchangeDbContext> options)
        : base(options) { }

    public DbSet<Parameter> Parameters => Set<Parameter>();
    public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ExchangeDbContext).Assembly);
    }
}
