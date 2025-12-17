namespace Dismissal_Appointment.Data;

public class DatabaseInitializer
{
    private readonly AppDbContext _context;

    public DatabaseInitializer(AppDbContext context)
    {
        _context = context;
    }

    public async Task InitializeAsync()
    {
        // Ensure the database is created
        await _context.Database.EnsureCreatedAsync();

        // Alternatively, use migrations:
        // await _context.Database.MigrateAsync();
    }
}
