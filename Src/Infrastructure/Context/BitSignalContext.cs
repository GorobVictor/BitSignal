using Core.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public class BitSignalContext(DbContextOptions<BitSignalContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseCamelCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}