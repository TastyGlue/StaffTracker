using System.Text.Json.Serialization;

namespace StaffTracker.Models;

public class AppSettings
{
    public string? Culture { get; set; }

    // Grid State saving
    public bool GridStateSortsSaving { get; set; }
    public bool GridStateFiltersSaving { get; set; }
    public bool GridStatePageSizeSaving { get; set; }
    public bool GridStatePageIndexSaving { get; set; }
    public bool GridStateHiddenColumnsSaving { get; set; }

    [JsonIgnore]
    public bool IsGridStateSavingEnabled =>
        GridStateSortsSaving ||
        GridStateFiltersSaving ||
        GridStatePageSizeSaving ||
        GridStatePageIndexSaving ||
        GridStateHiddenColumnsSaving;

    // Form Fields saving
    public bool FormCreateNew { get; set; }
    public bool FormFieldEntryDate { get; set; }
    public bool FormFieldCompany { get; set; }
    public bool FormFieldDivision { get; set; }
}
