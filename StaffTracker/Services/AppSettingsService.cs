using StaffTracker.Models;
using System.Text.Json;

namespace StaffTracker.Services;

public class AppSettingsService
{
    private readonly string _settingsFilePath;
    private readonly SemaphoreSlim _fileLock = new(1, 1);
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    private AppSettings _settings;
    private bool _initialized = false;

    public AppSettingsService()
    {
        // Use local user's AppData for per-user settings
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(appDataPath, "StaffTracker");
        Directory.CreateDirectory(appFolder); // Ensure directory exists
        _settingsFilePath = Path.Combine(appFolder, "app_settings.json");
        _settings = GetDefaultSettings();
    }

    private async Task EnsureInitializedAsync()
    {
        if (_initialized) return;

        await _initLock.WaitAsync().ConfigureAwait(false);
        try
        {
            // Double-check after acquiring lock
            if (_initialized) return;

            await _fileLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    // Load existing settings
                    var json = await File.ReadAllTextAsync(_settingsFilePath).ConfigureAwait(false);
                    var loadedSettings = JsonSerializer.Deserialize<AppSettings>(json, _jsonOptions);
                    _settings = loadedSettings ?? GetDefaultSettings();
                }
                else
                {
                    // Create default settings file
                    _settings = GetDefaultSettings();
                    await SaveToFileInternalAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                // If loading fails, use default settings
                string errorMessage = Utils.Utils.GetFullExceptionMessage(ex);
                Log.Error("Failed to load app settings: {ErrorMessage}", errorMessage);
                _settings = GetDefaultSettings();
            }
            finally
            {
                _fileLock.Release();
            }

            _initialized = true;
            Log.Information("App settings initialized successfully");
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task<AppSettings> GetAsync()
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        return _settings;
    }

    public async Task<AppSettings> UpdateAsync(AppSettings appSettings)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        _settings = appSettings;
        await SaveToFileAsync().ConfigureAwait(false);
        return _settings;
    }

    private async Task SaveToFileAsync()
    {
        await _fileLock.WaitAsync().ConfigureAwait(false);
        try
        {
            await SaveToFileInternalAsync().ConfigureAwait(false);
        }
        finally
        {
            _fileLock.Release();
        }
    }

    private async Task SaveToFileInternalAsync()
    {
        // This method assumes the lock is already held
        try
        {
            var json = JsonSerializer.Serialize(_settings, _jsonOptions);
            await File.WriteAllTextAsync(_settingsFilePath, json).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            string errorMessage = Utils.Utils.GetFullExceptionMessage(ex);
            Log.Error("Failed to save app settings: {ErrorMessage}", errorMessage);
        }
    }

    private static AppSettings GetDefaultSettings()
    {
        return new AppSettings
        {
            Culture = "bg-BG",
            GridStateSortsSaving = true,
            GridStateFiltersSaving = true,
            GridStatePageSizeSaving = true,
            GridStatePageIndexSaving = true,
            GridStateHiddenColumnsSaving = true,
            FormCreateNew = false,
            FormFieldEntryDate = false,
            FormFieldCompany = false,
            FormFieldDivision = false,
            ExportDefaultFileName = "staff_entries",
            ExportPreferredDownloadDestination = Utils.Utils.GetDefaultDownloadFolder()
        };
    }
}
