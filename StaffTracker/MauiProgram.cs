using StaffTracker;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using StaffTracker.Data;
using StaffTracker.Models;
using StaffTracker.Services;

namespace StaffTracker
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
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

            // Register SQLite database
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(DatabaseConfig.ConnectionString);
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            });

            // Localization
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources/Translations");
            builder.Services.AddSingleton<ILocalizationService, LocalizationService>();
            builder.Services.AddSingleton<MudLocalizer, ResXMudLocalizer>();

            // Register database initializer
            builder.Services.AddTransient<DatabaseInitializer>();

            // Register services
            builder.Services.AddSingleton<EntryGridStateService>();
            builder.Services.AddSingleton<AppSettingsService>();
            builder.Services.AddScoped<IPageTitleService, PageTitleService>();
            builder.Services.AddScoped<IEntryService<EntryBase>, EntryBaseService>();
            builder.Services.AddScoped<IEntryService<Appointment>, AppointmentService>();
            builder.Services.AddScoped<IEntryService<Dismissal>, DismissalService>();

            // Register Validators
            builder.Services.AddSingleton<IValidator<Appointment>, AppointmentValidator>();
            builder.Services.AddSingleton<IValidator<Dismissal>, DismissalValidator>();

            // Register pages
            builder.Services.AddSingleton<MainPage>();

            // Configure Serilog
            var logPath = Path.Combine(AppContext.BaseDirectory, "Logs", "log-.txt");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(
                    path: logPath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            // Add Serilog to the logging infrastructure
            builder.Logging.AddSerilog(Log.Logger, dispose: true);

            // Global exception handling
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var exception = (Exception)args.ExceptionObject;
                string errorMessage = Utils.Utils.GetFullExceptionMessage(exception);
                Log.Fatal("Unhandled exception in AppDomain: {ErrorMessage}", errorMessage);
            };
            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                string errorMessage = Utils.Utils.GetFullExceptionMessage(args.Exception.Flatten());
                Log.Fatal("Unobserved task exception: {ErrorMessage}", errorMessage);
                args.SetObserved(); // Prevents app crash
            };

#if WINDOWS
        builder.ConfigureLifecycleEvents(events =>  
        {  
            events.AddWindows(wndLifeCycleBuilder =>
            {
                wndLifeCycleBuilder.OnWindowCreated(window =>
                {
                    nint hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                    Microsoft.UI.WindowId myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
                    var _appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(myWndId);

                    var presenter = _appWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
                    if (presenter != null)
                    {
                        presenter.IsMinimizable = true;
                        presenter.IsResizable = true;
                        presenter.Maximize();
                    }
                });
            });  
        });
#endif

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
