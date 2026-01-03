namespace Dismissal_Appointment.Models.GridState;

public class EntryGridState
{
    public List<ColumnSortState> Sorts { get; set; } = [];
    public List<ColumnFilterState> Filters { get; set; } = [];
    public int PageSize { get; set; } = 10;
    public int PageIndex { get; set; } = 0;
    public List<string> HiddenColumns { get; set; } = [];

    public bool IsEmpty()
    {
        return Sorts.Count == 0 &&
               Filters.Count == 0 &&
               (PageSize == 5 || PageSize == 0) &&
               PageIndex == 0 &&
               HiddenColumns.Count == 0;
    }
}
