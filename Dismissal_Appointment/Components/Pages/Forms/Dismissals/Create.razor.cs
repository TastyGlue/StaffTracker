namespace Dismissal_Appointment.Components.Pages.Forms.Dismissals;

public partial class Create : ExtendedComponentBase
{
    [Inject] protected IEntryService<Dismissal> DismissalService { get; set; } = null!;
    protected Dismissal Model { get; set; } = new() { EntryType = EntryType.Dismissal };

    protected override async Task OnInitializedAsync()
    {
        base.OnInitialized();
        SetTitle(Localizer["Create Dismissal"]);
    }

    protected async Task ValidSubmitHandler()
    {
        try
        {
            await DismissalService.Add(Model);
            Notify(Localizer["Dismissal created successfully."], Severity.Success);
            NavManager.NavigateTo("/");
        }
        catch (Exception)
        {
            Notify(Localizer[$"Error creating dismissal."], Severity.Error);
        }
    }
}
