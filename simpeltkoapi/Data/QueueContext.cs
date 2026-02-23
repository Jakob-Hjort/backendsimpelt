using Microsoft.EntityFrameworkCore;
using simpeltkoapi.Models;

namespace simpeltkoapi.Data;

public class QueueContext : DbContext
{
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Queue> Queues => Set<Queue>();
    public DbSet<QueueEntry> QueueEntries => Set<QueueEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Teacher>()
            .HasMany<Queue>().WithMany();

        modelBuilder.Entity<User>().UseTptMappingStrategy();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Queues.db");
        base.OnConfiguring(optionsBuilder);
    }
}