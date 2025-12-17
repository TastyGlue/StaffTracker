using Dismissal_Appointment.Models;
using Microsoft.EntityFrameworkCore;

namespace Dismissal_Appointment.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

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

        // Configure required properties
        modelBuilder.Entity<EntryBase>()
            .Property(e => e.CompanyName)
            .IsRequired();

        modelBuilder.Entity<EntryBase>()
            .Property(e => e.IDN)
            .IsRequired();

        modelBuilder.Entity<EntryBase>()
            .Property(e => e.FirstName)
            .IsRequired();

        modelBuilder.Entity<EntryBase>()
            .Property(e => e.Surname)
            .IsRequired();

        modelBuilder.Entity<EntryBase>()
            .Property(e => e.LabourCodeArticle)
            .IsRequired();

        modelBuilder.Entity<Appointment>()
            .Property(a => a.Position)
            .IsRequired();
    }
}
