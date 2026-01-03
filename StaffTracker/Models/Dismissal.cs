namespace StaffTracker.Models;

public class Dismissal : EntryBase
{
    public int LabourCodeArticle { get; set; } = 1;
    public int? LabourCodeParagraph { get; set; }
    public int? LabourCodeItem { get; set; }
    public int? CompensationDays { get; set; }
    public bool? Garnishment { get; set; }
    public int? LeaveLastMonthDays { get; set; }
}
