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

            await databaseInitializer.InitializeAsync().ConfigureAwait(false);
            await databaseInitializer.SeedTestDataAsync().ConfigureAwait(false);
        }

        private async Task InitializeLocalizationAsync()
        {
            var appSettingsService = _serviceProvider.GetRequiredService<AppSettingsService>();
            var localization = _serviceProvider.GetRequiredService<ILocalizationService>();

            var appSettings = await appSettingsService.GetAsync().ConfigureAwait(false);
            if (appSettings != null && !string.IsNullOrEmpty(appSettings.Culture))
            {
                localization.SetCulture(appSettings.Culture);
            }
            else
            {
                // Default to Bulgarian (though GetAsync will return default settings)
                localization.SetCulture("bg-BG");
            }
        }
    }
}
