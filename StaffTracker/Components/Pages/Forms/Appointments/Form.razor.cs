using StaffTracker.Enums;
using StaffTracker.Models;

namespace StaffTracker.Components.Pages.Forms.Appointments;

public partial class Form : FormBase<Appointment>
{
    // Work Experience - Overall
    private int? workExpYears;
    private int? workExpMonths;
    private int? workExpDays;

    // Work Experience - In Profession
    private int? workExpProfYears;
    private int? workExpProfMonths;
    private int? workExpProfDays;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (IsCreate)
        {
            Model.Currency = Currency.EUR;
        }

        // Convert total days to years, months, days for display
        if (Model.WorkExperienceDays.HasValue)
        {
            ConvertDaysToYearsMonthsDays(Model.WorkExperienceDays, out workExpYears, out workExpMonths, out workExpDays);
        }

        if (Model.WorkExperienceInProfessionDays.HasValue)
        {
            ConvertDaysToYearsMonthsDays(Model.WorkExperienceInProfessionDays, out workExpProfYears, out workExpProfMonths, out workExpProfDays);
        }
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // Convert years, months, days back to total days before submission
        Model.WorkExperienceDays = ConvertYearsMonthsDaysToDays(workExpYears, workExpMonths, workExpDays);
        Model.WorkExperienceInProfessionDays = ConvertYearsMonthsDaysToDays(workExpProfYears, workExpProfMonths, workExpProfDays);
    }
}
