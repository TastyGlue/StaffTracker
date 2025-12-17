namespace Dismissal_Appointment.Models;

public class Appointment : EntryBase
{
    public decimal Salary { get; set; }
    public string Position { get; set; } = default!;
    public int? LengthOfServiceDays { get; set; }
    public string? IdCardNumber { get; set; }
    public DateTime? IdCardDate { get; set; }
}
