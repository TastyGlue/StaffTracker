namespace Dismissal_Appointment
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;

            // Initialize database synchronously before creating any pages
            InitializeDatabaseAsync().GetAwaiter().GetResult();

            // Initialize localization from saved settings
            InitializeLocalizationAsync().GetAwaiter().GetResult();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var mainPage = _serviceProvider.GetRequiredService<MainPage>();
            return new Window(mainPage) { Title = "Dismissal_Appointment" };
        }

        private async Task InitializeDatabaseAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();

            await databaseInitializer.InitializeAsync();
            await databaseInitializer.SeedAppSettings();
            await databaseInitializer.SeedTestDataAsync();
        }

        private async Task InitializeLocalizationAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var localization = _serviceProvider.GetRequiredService<ILocalizationService>();

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
        }
    }
}
