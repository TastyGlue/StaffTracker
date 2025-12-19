namespace Dismissal_Appointment.Components.Pages.EntryLists;

public partial class All : EntryListBase<EntryBase>
{
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
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
}
