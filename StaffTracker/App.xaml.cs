using StaffTracker.Data;
using StaffTracker.Services;

namespace StaffTracker
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;

            Log.Information("Application starting...");

            // Initialize database synchronously before creating any pages
            InitializeDatabaseAsync().GetAwaiter().GetResult();

            // Initialize localization from saved settings
            InitializeLocalizationAsync().GetAwaiter().GetResult();

            Log.Information("Application initialized successfully");
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var mainPage = _serviceProvider.GetRequiredService<MainPage>();
            return new Window(mainPage) { Title = "StaffTracker", MinimumWidth = 625 };
        }

        private async Task InitializeDatabaseAsync()
        {
            Log.Information("Initializing database...");
            using var scope = _serviceProvider.CreateScope();
            var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();

            await databaseInitializer.InitializeAsync().ConfigureAwait(false);
            //await databaseInitializer.SeedTestDataAsync().ConfigureAwait(false);
            Log.Information("Database initialized successfully");
        }

        private async Task InitializeLocalizationAsync()
        {
            var appSettingsService = _serviceProvider.GetRequiredService<AppSettingsService>();
            var localization = _serviceProvider.GetRequiredService<ILocalizationService>();

            var appSettings = await appSettingsService.GetAsync().ConfigureAwait(false);
            if (appSettings != null && !string.IsNullOrEmpty(appSettings.Culture))
            {
                Log.Information("Initial culture set to {Culture}", appSettings.Culture);
                localization.SetCulture(appSettings.Culture);
            }
            else
            {
                // Default to Bulgarian (though GetAsync will return default settings)
                Log.Information("Initial culture set to default (bg-BG)");
                localization.SetCulture("bg-BG");
            }
            Log.Information("Localization initialized successfully");
        }
    }
}
