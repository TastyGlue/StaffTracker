using StaffTracker.Models.GridState;
using System.Text.Json;

namespace StaffTracker.Services;

public class EntryGridStateService
{
    private readonly string _stateFilePath;
    private readonly SemaphoreSlim _fileLock = new(1, 1);
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    private EntryGridState _gridState;
    private bool _initialized = false;

    public EntryGridState GridState
    {
        get => _gridState;
        private set => _gridState = value;
    }

    public async Task<EntryGridState> GetGridStateAsync()
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        return _gridState;
    }

    public EntryGridStateService()
    {
        // Use local user's AppData for per-user grid preferences
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(appDataPath, "StaffTracker");
        Directory.CreateDirectory(appFolder); // Ensure directory exists
        _stateFilePath = Path.Combine(appFolder, "entry_grid_state.json");
        _gridState = new EntryGridState();
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
                if (File.Exists(_stateFilePath))
                {
                    // Load existing state
                    var json = await File.ReadAllTextAsync(_stateFilePath).ConfigureAwait(false);
                    var loadedState = JsonSerializer.Deserialize<EntryGridState>(json, _jsonOptions);
                    _gridState = loadedState ?? new EntryGridState();
                }
                else
                {
                    // Create empty state file
                    _gridState = new EntryGridState();
                    await SaveToFileInternalAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = Utils.Utils.GetFullExceptionMessage(ex);
                Log.Error("Failed to load entry grid state: {ErrorMessage}", errorMessage);
                
                // If loading fails, use default state
                _gridState = new EntryGridState();
            }
            finally
            {
                _fileLock.Release();
            }

            _initialized = true;
            Log.Information("Entry grid state initialized successfully");
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task UpdateSortsAsync(List<ColumnSortState> sorts)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        _gridState.Sorts = sorts;
        await SaveToFileAsync().ConfigureAwait(false);
    }

    public async Task UpdateFiltersAsync(List<ColumnFilterState> filters)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        _gridState.Filters = filters;
        await SaveToFileAsync().ConfigureAwait(false);
    }

    public async Task UpdatePagingAsync(int pageSize, int pageIndex)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        _gridState.PageSize = pageSize;
        _gridState.PageIndex = pageIndex;
        await SaveToFileAsync().ConfigureAwait(false);
    }

    public async Task UpdateHiddenColumnsAsync(List<string> hiddenColumns)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        _gridState.HiddenColumns = hiddenColumns;
        await SaveToFileAsync().ConfigureAwait(false);
    }

    public async Task UpdateFullStateAsync(EntryGridState state)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        _gridState = state;
        await SaveToFileAsync().ConfigureAwait(false);
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
            var json = JsonSerializer.Serialize(_gridState, _jsonOptions);
            await File.WriteAllTextAsync(_stateFilePath, json).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            var errorMessage = Utils.Utils.GetFullExceptionMessage(ex);
            Log.Error("Failed to save entry grid state: {ErrorMessage}", errorMessage);
        }
    }
}
