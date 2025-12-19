using Microsoft.EntityFrameworkCore;

namespace Dismissal_Appointment.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<EntryBase> Entries { get; set; } = null!;
    public DbSet<Appointment> Appointments { get; set; } = null!;
    public DbSet<Dismissal> Dismissals { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the base class hierarchy
        modelBuilder.Entity<Appointment>()
            .HasBaseType<EntryBase>();

        modelBuilder.Entity<Dismissal>()
            .HasBaseType<EntryBase>();

        // Configure EntryBase
        modelBuilder.Entity<EntryBase>()
            .UseTptMappingStrategy(); // Table-per-type mapping

        // Configure decimal precision for Salary
        modelBuilder.Entity<Appointment>()
            .Property(a => a.Salary)
            .HasPrecision(18, 2);
    }
}
