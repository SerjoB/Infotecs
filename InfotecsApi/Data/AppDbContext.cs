using InfotecsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InfotecsApi.Data;

public class AppDbContext: DbContext
{
    public DbSet<ValueModel> Values => Set<ValueModel>();
    public DbSet<ResultModel> Results => Set<ResultModel>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}