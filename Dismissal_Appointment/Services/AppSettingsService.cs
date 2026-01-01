namespace Dismissal_Appointment.Services;

public class AppSettingsService
{
    private readonly AppDbContext _context;
    private readonly AppSettingsStateContainer _stateContainer;

    public AppSettingsService(AppDbContext context, AppSettingsStateContainer stateContainer)
    {
        _context = context;
        _stateContainer = stateContainer;
    }

    public async Task<AppSettings?> GetAsync()
    {
        var cached = await _stateContainer.GetCachedAsync();
        if (cached == null)
        {
            cached = await _context.AppSettings.FirstOrDefaultAsync();
            if (cached != null)
            {
                await _stateContainer.SetCachedAsync(cached);
            }
        }
        return cached;
    }

    public async Task<AppSettings> UpdateAsync(AppSettings appSettings)
    {
        _context.ChangeTracker.Clear();
        _context.AppSettings.Update(appSettings);
        await _context.SaveChangesAsync();
        await _stateContainer.SetCachedAsync(appSettings);
        return appSettings;
    }
}
