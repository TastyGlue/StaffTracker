using Dismissal_Appointment.Models.GridState;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Dismissal_Appointment.Components.Pages.EntryLists;

public partial class All : EntryListBase<EntryBase>
{
    [Inject] protected IJSRuntime JS { get; set; } = default!;
    [Inject] protected EntryGridStateService GridStateService { get; set; } = default!;
    [Inject] protected ILogger<All> Logger { get; set; } = default!;
    [Inject] protected AppSettingsService AppSettingsService { get; set; } = default!;

    protected MudDataGrid<EntryBase> DataGrid { get; set; } = default!;
    protected int CurrentPage { get; set; }
    protected string EntryTypeColFilterOperator => Localizer[FilterOperator.Enum.Is];
    protected EntryType? EntryTypeColFilterValue { get; set; }
    protected Func<EntryType?, string?> EntryTypeToString =>
        (entryType) => entryType.HasValue ? Localizer[entryType.Value.ToString()] : null;
    protected FilterDefinition<EntryBase> EntryTypeFilterDef { get; set; } = default!;

    // Grid state tracking fields
    private Timer? _autoSaveTimer;
    private int _lastSortCount = 0;
    private int _lastFilterCount = 0;
    private int _lastPageSize = 10;
    private int _lastPageIndex = 0;
    private HashSet<int> _lastHiddenColumns = [];

    // Must be kept in sync with number of columns in the grid
    private bool[] _hiddenCols = new bool[11];

    #region Filter Operators
    protected readonly HashSet<string> NonNullableStringFilterOperators =
    [
        FilterOperator.String.Contains,
        FilterOperator.String.NotContains,
        FilterOperator.String.Equal,
        FilterOperator.String.NotEqual,
        FilterOperator.String.StartsWith,
        FilterOperator.String.EndsWith
    ];

    protected readonly HashSet<string> NonNullableIntFilterOperators =
    [
        FilterOperator.Number.Equal,
        FilterOperator.Number.NotEqual,
        FilterOperator.Number.GreaterThan,
        FilterOperator.Number.GreaterThanOrEqual,
        FilterOperator.Number.LessThan,
        FilterOperator.Number.LessThanOrEqual
    ];

    protected readonly HashSet<string> NonNullableDateFilterOperators =
    [
        FilterOperator.DateTime.Is,
        FilterOperator.DateTime.IsNot,
        FilterOperator.DateTime.After,
        FilterOperator.DateTime.OnOrAfter,
        FilterOperator.DateTime.Before,
        FilterOperator.DateTime.OnOrBefore
    ];
    #endregion

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var entries = await EntriesService.GetAll();
        Entries = new(entries);

        IsLoading = false;

        SetTitle("Entries");

        // Subscribe to navigation events to save state when navigating away
        NavManager.LocationChanged += OnLocationChanged;
    }

    private async void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        // Save state asynchronously when navigating away
        //try
        //{
        //    await SaveGridStateAsync();
        //}
        //catch (Exception ex)
        //{
        //    Logger.LogError(ex, "Failed to save grid state on navigation");
        //}
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (firstRender)
        {
            await JS.InvokeVoidAsync("setTableContainerMaxHeight");

            var entryTypeCol = DataGrid.GetColumnByPropertyName(nameof(EntryBase.EntryType))!;
            EntryTypeFilterDef = new FilterDefinition<EntryBase>
            {
                Column = entryTypeCol,
                Title = entryTypeCol.Title,
                Operator = FilterOperator.Enum.Is,
            };

            await LoadGridStateAsync();

            // Start auto-save timer (checks for changes every 2 seconds)
            _autoSaveTimer = new Timer(_ =>
            {
                _ = InvokeAsync(CheckAndSaveState);
            }, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
        }
    }

    private async Task LoadGridStateAsync()
    {
        var appSettings = await AppSettingsService.GetAsync();
        if (appSettings != null && !appSettings.IsGridStateSavingEnabled)
            return;

        var savedState = await GridStateService.GetGridStateAsync();
        if (savedState.IsEmpty())
            return;

        // Load Sorts
        if (savedState.Sorts.Count != 0 &&
            (appSettings != null && appSettings.GridStateSortsSaving))
        {
            foreach (var sort in savedState.Sorts.OrderBy(s => s.Index))
            {
                var sortFunc = Utils.Utils.CreatePropertySelector(sort.PropertyName);
                await DataGrid.SetSortAsync(sort.PropertyName, sort.Direction, sortFunc);
            }
        }

        // Load Paging
        if (savedState.PageSize > 0 &&
            (appSettings != null && appSettings.GridStatePageSizeSaving))
        {
            await DataGrid.SetRowsPerPageAsync(savedState.PageSize);
            CurrentPage = savedState.PageIndex;
        }

        // Load Hidden Columns
        if (savedState.HiddenColumns.Count != 0 &&
            (appSettings != null && appSettings.GridStateHiddenColumnsSaving))
        {
            foreach (var col in DataGrid.RenderedColumns)
            {
                if (savedState.HiddenColumns.Contains(col.PropertyName ?? ""))
                    await col.HideAsync();
            }
        }

        // Load Filters
        if (savedState.Filters.Count != 0 && 
            (appSettings != null && appSettings.GridStateFiltersSaving))
        {
            foreach (var filter in savedState.Filters)
            {
                if (filter.PropertyName == nameof(EntryBase.EntryType))
                {
                    var context = DataGrid.GetColumnByPropertyName(filter.PropertyName)!.FilterContext;
                    EntryTypeColFilterValue = Utils.Utils.TryGetEnumValue<EntryType>(Convert.ToInt32(filter.Value), out var entryType)
                        ? entryType
                        : null;
                    await ApplyEntryTypeFilterAsync(context);
                    continue;
                }

                var column = DataGrid.GetColumnByPropertyName(filter.PropertyName)!;
                var filterDef = new FilterDefinition<EntryBase>()
                {
                    Column = column,
                    Title = column.Title,
                    Operator = filter.Operator,
                    Value = filter.Value
                };

                column.FilterContext.FilterDefinition = filterDef;
                DataGrid.FilterDefinitions.Add(filterDef);
            }
        }

        // Trigger grid refresh
        await InvokeAsync(StateHasChanged);
    }

    private async Task CheckAndSaveState()
    {
        if (DataGrid == null)
            return;

        var sortCount = DataGrid.SortDefinitions.Count;
        var filterCount = DataGrid.FilterDefinitions.Count;
        var pageSize = DataGrid.RowsPerPage;
        var pageIndex = DataGrid.CurrentPage;
        var hiddenCols = _hiddenCols
            .Where((hidden) => hidden)
            .Select((hidden, index) => index)
            .ToHashSet();

        if (sortCount != _lastSortCount ||
            filterCount != _lastFilterCount ||
            pageSize != _lastPageSize ||
            pageIndex != _lastPageIndex ||
            !hiddenCols.SetEquals(_lastHiddenColumns))
        {
            _lastSortCount = sortCount;
            _lastFilterCount = filterCount;
            _lastPageSize = pageSize;
            _lastPageIndex = pageIndex;
            _lastHiddenColumns = hiddenCols;

            await SaveGridStateAsync();
        }
    }

    private async Task SaveGridStateAsync()
    {
        if (DataGrid == null)
            return;

        var state = new EntryGridState
        {
            Sorts = DataGrid.SortDefinitions.Select(x => new ColumnSortState
            {
                PropertyName = x.Value.SortBy,
                Direction = x.Value.Descending ? SortDirection.Descending : SortDirection.Ascending,
                Index = x.Value.Index
            }).ToList(),

            Filters = DataGrid.FilterDefinitions
                .Where(f => f.Column?.PropertyName != null)
                .Select(f => new ColumnFilterState
                {
                    PropertyName = f.Column!.PropertyName,
                    Operator = f.Operator ?? "",
                    Value = f.Value
                }).ToList(),

            PageSize = DataGrid.RowsPerPage,
            PageIndex = CurrentPage,

            HiddenColumns = DataGrid.RenderedColumns
                .Where(c => c.Hidden && c.PropertyName != null)
                .Select(c => c.PropertyName!)
                .ToList()
        };

        await GridStateService.UpdateFullStateAsync(state);
    }

    protected void NavigateToEditEntry()
    {
        string entryType = SelectedEntry!.EntryType.ToString().ToLower();
        NavManager.NavigateTo($"/{entryType}/edit/{SelectedEntry.Id}");
    }

    protected async Task DeleteEntry()
    {
        if (SelectedEntry is null)
            return;

        var parameters = new DialogParameters { ["Entry"] = SelectedEntry };
        var options = new DialogOptions { MaxWidth = MaxWidth.Large, BackdropClick = true, CloseOnEscapeKey = true, Position = DialogPosition.Center };
        var dialog = await DialogService.ShowAsync<ConfirmDeleteDialog>(null, parameters, options);

        var result = await dialog.Result;
        if (result is not null && !result.Canceled)
        {
            Entries.Remove(SelectedEntry);
            SelectedEntry = null;
        }
    }

    #region Grid Events
    private async Task ClearEntryTypeFilterAsync(FilterContext<EntryBase> context)
    {
        EntryTypeColFilterValue = null;
        await context.Actions.ClearFilterAsync(EntryTypeFilterDef);
    }

    private async Task ApplyEntryTypeFilterAsync(FilterContext<EntryBase> context)
    {
        EntryTypeFilterDef.Value = EntryTypeColFilterValue;
        await context.Actions.ApplyFilterAsync(EntryTypeFilterDef);
    }

    private bool QuickFilter(EntryBase entry)
    {
        if (string.IsNullOrWhiteSpace(SearchString))
            return true;

        return (entry.Id.ToString().Contains(SearchString, StringComparison.OrdinalIgnoreCase))
            || (entry.CompanyName.Contains(SearchString, StringComparison.OrdinalIgnoreCase))
            || (entry.Division?.Contains(SearchString, StringComparison.OrdinalIgnoreCase) ?? false)
            || (entry.IDN.Contains(SearchString, StringComparison.OrdinalIgnoreCase))
            || (entry.FullName.Contains(SearchString, StringComparison.OrdinalIgnoreCase));
    }
    #endregion

    public override void Dispose()
    {
        // Stop and dispose timer
        _autoSaveTimer?.Dispose();

        // Unsubscribe from navigation events
        NavManager.LocationChanged -= OnLocationChanged;

        // Save final state before component is destroyed (works for in-app navigation)
        // Use Task.Run to avoid UI thread deadlock
        try
        {
            Task.Run(async () => await SaveGridStateAsync()).Wait();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to save grid state on dispose");
        }

        GC.SuppressFinalize(this);
        base.Dispose();
    }
}
