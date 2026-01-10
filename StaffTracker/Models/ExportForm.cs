namespace StaffTracker.Models;

public class ExportForm
{
    public ExportType ExportType { get; set; }
    public DateTime? Day { get; set; }
    public DateTime? Month { get; set; }
    public DateTime? Year { get; set; }
    public DateTime? RangeStartDate { get; set; }
    public DateTime? RangeEndDate { get; set; }
    public string Folder { get; set; } = default!;
    public string? FileName { get; set; }
    public string? Company { get; set; }
}
