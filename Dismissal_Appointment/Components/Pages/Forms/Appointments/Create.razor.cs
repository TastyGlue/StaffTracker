namespace Dismissal_Appointment.Components.Pages.Forms.Appointments;

public partial class Create : ExtendedComponentBase
{
    [Inject] protected IEntryService<Appointment> AppointmentService { get; set; } = null!;
    [Inject] protected AppSettingsService AppSettingsService { get; set; } = null!;
    protected Appointment Model { get; set; } = new() { EntryType = EntryType.Appointment };

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

            var appSettings = await AppSettingsService.GetAsync();
            if (appSettings is not null && appSettings.FormCreateNew)
            {
                var previousEntryDate = Model.EntryDate;
                var previousCompanyName = Model.CompanyName;
                var previousDivision = Model.Division;

                // Reset the form for new entry
                Model = new Appointment 
                { 
                    EntryType = EntryType.Appointment,
                    Currency = Currency.BGN
                };

                if (appSettings.FormFieldEntryDate)
                    Model.EntryDate = previousEntryDate;

                if (appSettings.FormFieldCompany)
                    Model.CompanyName = previousCompanyName;

                if (appSettings.FormFieldDivision)
                    Model.Division = previousDivision;
            }
            else
                NavManager.NavigateTo("/");
        }
        catch (Exception)
        {
            Notify(Localizer[$"Error creating appointment."], Severity.Error);
        }
    }
}
