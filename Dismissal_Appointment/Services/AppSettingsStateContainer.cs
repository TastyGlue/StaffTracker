namespace Dismissal_Appointment.Services;

public class AppSettingsStateContainer
{
    private AppSettings? _cachedSettings;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public async Task<AppSettings?> GetCachedAsync()
    {
        await _lock.WaitAsync();
        try
        {
            return _cachedSettings;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task SetCachedAsync(AppSettings? settings)
    {
        await _lock.WaitAsync();
        try
        {
            _cachedSettings = settings;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task ClearCacheAsync()
    {
        await _lock.WaitAsync();
        try
        {
            _cachedSettings = null;
        }
        finally
        {
            _lock.Release();
        }
    }
}
