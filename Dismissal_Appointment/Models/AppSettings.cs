using System.ComponentModel.DataAnnotations.Schema;

namespace Dismissal_Appointment.Models;

public class AppSettings
{
    public int Id { get; set; }

    public string? Culture { get; set; }

    // Grid State saving
    public bool GridStateSortsSaving { get; set; }
    public bool GridStateFiltersSaving { get; set; }
    public bool GridStatePageSizeSaving { get; set; }
    public bool GridStatePageIndexSaving { get; set; }
    public bool GridStateHiddenColumnsSaving { get; set; }

    [NotMapped]
    public bool IsGridStateSavingEnabled =>
        GridStateSortsSaving ||
        GridStateFiltersSaving ||
        GridStatePageSizeSaving ||
        GridStatePageIndexSaving ||
        GridStateHiddenColumnsSaving;

    // Form Fields saving
    public bool FormCreateNew { get; set; }
    public bool FormFieldEntryDate { get; set; }
    //public DateTime? FormFieldEntryDateValue { get; set; }
    public bool FormFieldCompany { get; set; }
    //public string? FormFieldCompanyValue { get; set; }
    public bool FormFieldDivision { get; set; }
    //public string? FormFieldDivisionValue { get; set; }
}
