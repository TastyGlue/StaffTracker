using System.Collections.ObjectModel;

namespace Dismissal_Appointment.Components.Pages.Abstract;

public partial class EntryListBase<T> : ExtendedComponentBase
    where T : EntryBase, new()
{
    [Inject] protected IEntryService<T> EntriesService { get; set; } = default!;

    protected ObservableCollection<T> Entries { get; set; } = [];
    protected T? SelectedEntry { get; set; } = null;
    protected bool IsLoading { get; set; }
    public string SearchString { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        base.OnInitialized();

        IsLoading = true;

        var entries = await EntriesService.GetAll();
        Entries = new(entries);

        IsLoading = false;
    }

    protected Func<T, int, string> RowClassFunc => (x, i) =>
    {
        var classes = new List<string>();

        if (SelectedEntry is not null && x.Id == SelectedEntry.Id)
        {
            classes.Add("table-hover-color");
            //classes.Add("row-ripple-effect");
        }

        return string.Join(" ", classes);
    };
}
