using StaffTracker.Models;
using StaffTracker.Services;

namespace StaffTracker.Components.Pages.Shared;

public partial class ConfirmDeleteDialog : ExtendedComponentBase
{
    [Inject] protected IEntryService<EntryBase> EntriesService { get; set; } = default!;

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public EntryBase Entry { get; set; } = new();
    [Parameter] public bool IsCardVisible { get; set; } = true;

    private void Cancel() => MudDialog.Cancel();

    private async Task DeleteEntry()
    {
        var result = await EntriesService.Delete(Entry.Id);
        if (!result)
        {
            Notify("An error occurred while deleting the entry.", Severity.Error);
            MudDialog.Cancel();
            return;
        }

        Notify("Entry deleted successfully.", Severity.Success);
        MudDialog.Close(DialogResult.Ok(Entry.Id));
    }
}
