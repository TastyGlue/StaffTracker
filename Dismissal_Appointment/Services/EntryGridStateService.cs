using Dismissal_Appointment.Models.GridState;
using System.Text.Json;

namespace Dismissal_Appointment.Services;

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
        await EnsureInitializedAsync();
        return _gridState;
    }

    public EntryGridStateService()
    {
        _stateFilePath = Path.Combine(AppContext.BaseDirectory, "entry_grid_state.json");
        _gridState = new EntryGridState();
    }

    private async Task EnsureInitializedAsync()
    {
        if (_initialized) return;

        await _initLock.WaitAsync();
        try
        {
            // Double-check after acquiring lock
            if (_initialized) return;

            await _fileLock.WaitAsync();
            try
            {
                if (File.Exists(_stateFilePath))
                {
                    // Load existing state
                    var json = await File.ReadAllTextAsync(_stateFilePath);
                    var loadedState = JsonSerializer.Deserialize<EntryGridState>(json, _jsonOptions);
                    _gridState = loadedState ?? new EntryGridState();
                }
                else
                {
                    // Create empty state file
                    _gridState = new EntryGridState();
                    await SaveToFileInternalAsync();
                }
            }
            catch (Exception ex)
            {
                // If loading fails, use default state
                Console.WriteLine($"Failed to load grid state: {ex.Message}");
                _gridState = new EntryGridState();
            }
            finally
            {
                _fileLock.Release();
            }

            _initialized = true;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task UpdateSortsAsync(List<ColumnSortState> sorts)
    {
        await EnsureInitializedAsync();
        _gridState.Sorts = sorts;
        await SaveToFileAsync();
    }

    public async Task UpdateFiltersAsync(List<ColumnFilterState> filters)
    {
        await EnsureInitializedAsync();
        _gridState.Filters = filters;
        await SaveToFileAsync();
    }

    public async Task UpdatePagingAsync(int pageSize, int pageIndex)
    {
        await EnsureInitializedAsync();
        _gridState.PageSize = pageSize;
        _gridState.PageIndex = pageIndex;
        await SaveToFileAsync();
    }

    public async Task UpdateHiddenColumnsAsync(List<string> hiddenColumns)
    {
        await EnsureInitializedAsync();
        _gridState.HiddenColumns = hiddenColumns;
        await SaveToFileAsync();
    }

    public async Task UpdateFullStateAsync(EntryGridState state)
    {
        await EnsureInitializedAsync();
        _gridState = state;
        await SaveToFileAsync();
    }

    private async Task SaveToFileAsync()
    {
        await _fileLock.WaitAsync();
        try
        {
            await SaveToFileInternalAsync();
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
            await File.WriteAllTextAsync(_stateFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save grid state: {ex.Message}");
        }
    }
}
