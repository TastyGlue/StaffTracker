namespace Dismissal_Appointment.Models;

public class Dismissal : EntryBase
{
    public string LabourCodeArticle { get; set; } = default!;
    public int? CompensationDays { get; set; }
    public bool? Garnishment { get; set; }
    public int? LeaveLastMonthDays { get; set; }
}
