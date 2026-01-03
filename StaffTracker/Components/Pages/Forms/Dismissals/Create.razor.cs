using StaffTracker.Enums;
using StaffTracker.Models;
using StaffTracker.Services;

namespace StaffTracker.Components.Pages.Forms.Dismissals;

public partial class Create : ExtendedComponentBase
{
    [Inject] protected IEntryService<Dismissal> DismissalService { get; set; } = null!;
    [Inject] protected AppSettingsService AppSettingsService { get; set; } = null!;
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

            var appSettings = await AppSettingsService.GetAsync();
            if (appSettings is not null && appSettings.FormCreateNew)
            {
                var previousEntryDate = Model.EntryDate;
                var previousCompanyName = Model.CompanyName;
                var previousDivision = Model.Division;

                // Reset the form for new entry
                Model = new Dismissal
                {
                    EntryType = EntryType.Dismissal
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
            Notify(Localizer[$"Error creating dismissal."], Severity.Error);
        }
    }
}
