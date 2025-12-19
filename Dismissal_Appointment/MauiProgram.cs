using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;
using Dismissal_Appointment.Services;

namespace Dismissal_Appointment
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            // Set default culture to Bulgarian
            var culture = new CultureInfo("bg-BG");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

            // Add UI Component Library
            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = false;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.HideTransitionDuration = 500;
                config.SnackbarConfiguration.ShowTransitionDuration = 500;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            });
            builder.Services.AddMudLocalization();

            // Register SQLite database
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(DatabaseConfig.ConnectionString));

            // Localization
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources/Translations");
            builder.Services.AddSingleton<ILocalizationService, LocalizationService>();

            // Register database initializer
            builder.Services.AddTransient<DatabaseInitializer>();

            // Register services
            builder.Services.AddScoped<IPageTitleService, PageTitleService>();
            builder.Services.AddScoped<IEntryService<Appointment>, AppointmentService>();
            builder.Services.AddScoped<IEntryService<Dismissal>, DismissalService>();

            // Register pages
            builder.Services.AddSingleton<MainPage>();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
