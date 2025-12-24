using Microsoft.JSInterop;

namespace Dismissal_Appointment.Components.Pages.EntryLists;

public partial class All : EntryListBase<EntryBase>
{
    [Inject] protected IJSRuntime JS { get; set; } = default!;
    [Inject] public IDialogService DialogService { get; set; } = default!;

    protected MudDataGrid<EntryBase> DataGrid { get; set; } = default!;
    protected string EntryTypeColFilterOperator => Localizer[FilterOperator.Enum.Is];
    protected EntryType? EntryTypeColFilterValue { get; set; }
    protected Func<EntryType?, string?> EntryTypeToString =>
        (entryType) => entryType.HasValue ? Localizer[entryType.Value.ToString()] : null;
    protected FilterDefinition<EntryBase> EntryTypeFilterDef { get; set; } = default!;

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

        SetTitle("Entries");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (firstRender)
        {
            await JS.InvokeVoidAsync("setTableContainerMaxHeight");

            EntryTypeFilterDef = new FilterDefinition<EntryBase>
            {
                FilterFunction = x => !EntryTypeColFilterValue.HasValue || x.EntryType == EntryTypeColFilterValue.Value,
                Column = DataGrid.GetColumnByPropertyName(nameof(EntryBase.EntryType)),
                Title = Localizer["Entry Type"],
                Operator = FilterOperator.Enum.Is,
            };
        }
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
    private async Task ClearFilterAsync(FilterContext<EntryBase> context)
    {
        EntryTypeColFilterValue = null;
        await context.Actions.ClearFilterAsync(EntryTypeFilterDef);
    }

    private async Task ApplyFilterAsync(FilterContext<EntryBase> context)
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
}
