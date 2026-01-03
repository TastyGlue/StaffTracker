namespace StaffTracker.Data;

public static class DatabaseConfig
{
    public static string DatabasePath
    {
        get
        {
            var databaseFolder = Path.Combine(AppContext.BaseDirectory, "Database");

            // Ensure the directory exists
            Directory.CreateDirectory(databaseFolder);

            return Path.Combine(databaseFolder, "dismissal_appointment.db");
        }
    }

    public static string ConnectionString => $"Data Source={DatabasePath}";
}
