namespace Dismissal_Appointment.Data;

public static class DatabaseConfig
{
    public static string DatabasePath
    {
        get
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var databaseFolder = Path.Combine(appDataPath, "DismissalAppointment");

            // Ensure the directory exists
            Directory.CreateDirectory(databaseFolder);

            return Path.Combine(databaseFolder, "dismissal_appointment.db");
        }
    }

    public static string ConnectionString => $"Data Source={DatabasePath}";
}
