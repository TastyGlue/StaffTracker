namespace Dismissal_Appointment
{
    public partial class MainPage : ContentPage
    {
        public MainPage(DatabaseInitializer databaseInitializer, AppDbContext context, ILocalizationService localization)
        {
            InitializeComponent();

            // Initialize the database
            Task.Run(async () =>
            {
                await databaseInitializer.InitializeAsync();
                await databaseInitializer.SeedAppSettings();
                await databaseInitializer.SeedTestDataAsync();
            });

            // Set localization
            Task.Run(async () =>
            {
                var appSettings = await context.AppSettings.FirstOrDefaultAsync();
                if (appSettings != null && !string.IsNullOrEmpty(appSettings.Culture))
                {
                    localization.SetCulture(appSettings.Culture);
                }
                else
                {
                    // Default to Bulgarian
                    localization.SetCulture("bg-BG");
                }
            });
        }
    }
}
