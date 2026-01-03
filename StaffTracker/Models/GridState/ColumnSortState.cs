namespace StaffTracker.Models.GridState;

public class ColumnSortState
{
    public string PropertyName { get; set; } = default!;
    public SortDirection Direction { get; set; }
    public int Index { get; set; }
}
