namespace Dismissal_Appointment.Components.Pages.Forms.Appointments;

public partial class Create : ExtendedComponentBase
{
    [Inject] protected IEntryService<Appointment> AppointmentService { get; set; } = null!;
    protected Appointment Model { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        base.OnInitialized();
        SetTitle(Localizer["Create Appointment"]);
    }

    protected async Task ValidSubmitHandler()
    {
        try
        {
            await AppointmentService.Add(Model);
            Notify(Localizer["Appointment created successfully."], Severity.Success);
            NavManager.NavigateTo("/");
        }
        catch (Exception)
        {
            Notify(Localizer[$"Error creating appointment."], Severity.Error);
        }
    }
}
