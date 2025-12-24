namespace Dismissal_Appointment.Components.Pages.Forms.Appointments;

public partial class Edit : ExtendedComponentBase
{
    [Inject] protected IEntryService<Appointment> AppointmentService { get; set; } = null!;
    [Parameter] public int Id { get; set; }
    protected Appointment Model { get; set; } = new();
    protected bool IsLoading { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        base.OnInitialized();

        try
        {
            var model = await AppointmentService.GetById(Id);
            if (model is null)
            {
                Notify(Localizer["Error loading appointment."], Severity.Error);
                NavManager.NavigateTo("/");
                return;
            }
            Model = model;
        }
        catch (Exception)
        {
            Notify(Localizer["Error loading appointment."], Severity.Error);
        }

        IsLoading = false;

        SetTitle(Localizer["Edit Appointment №{0}", Id]);
    }

    protected async Task ValidSubmitHandler()
    {
        try
        {
            await AppointmentService.Update(Model);
            Notify(Localizer["Appointment edited successfully."], Severity.Success);
            NavManager.NavigateTo("/");
        }
        catch (Exception)
        {
            Notify(Localizer[$"Error editing appointment."], Severity.Error);
        }
    }
}
