namespace StaffTracker.Models;

public class ExportForm
{
    public ExportType ExportType { get; set; }
    public DateTime? Day { get; set; }
    public DateTime? Year { get; set; }
    public string Folder { get; set; } = default!;
    public string FileName { get; set; } = default!;
}
