namespace Dismissal_Appointment.Components.Pages.Abstract;

public partial class EntryListBase<T> : ExtendedComponentBase
    where T : EntryBase, new()
{
    protected IEnumerable<T> Entries { get; set; } = [];
    protected T? SelectedEntry { get; set; } = null;
    protected bool IsLoading { get; set; }
    public string SearchString { get; set; } = string.Empty;

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }
}
