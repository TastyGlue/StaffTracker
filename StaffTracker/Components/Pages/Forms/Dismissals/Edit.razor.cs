using StaffTracker.Models;
using StaffTracker.Services;

namespace StaffTracker.Components.Pages.Forms.Dismissals;

public partial class Edit : ExtendedComponentBase
{
    [Inject] protected IEntryService<Dismissal> DismissalService { get; set; } = null!;
    [Parameter] public int Id { get; set; }
    protected Dismissal Model { get; set; } = new();
    protected bool IsLoading { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        base.OnInitialized();

        try
        {
            var model = await DismissalService.GetById(Id);
            if (model is null)
            {
                Notify(Localizer["Error loading dismissal."], Severity.Error);
                NavManager.NavigateTo("/");
                return;
            }
            Model = model;
        }
        catch (Exception)
        {
            Notify(Localizer["Error loading dismissal."], Severity.Error);
        }

        IsLoading = false;

        SetTitle(Localizer["Edit Dismissal №{0}", Id]);
    }

    protected async Task ValidSubmitHandler()
    {
        try
        {
            await DismissalService.Update(Model);
            Notify(Localizer["Dismissal edited successfully."], Severity.Success);
            NavManager.NavigateTo("/");
        }
        catch (Exception)
        {
            Notify(Localizer[$"Error editing dismissal."], Severity.Error);
        }
    }
}
