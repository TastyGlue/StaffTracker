namespace Dismissal_Appointment.Models;

public class Appointment : EntryBase
{
    public decimal Salary { get; set; }
    public Currency Currency { get; set; }
    public string Position { get; set; } = default!;
    public int? WorkExperienceDays { get; set; }
    public int? WorkExperienceInProfessionDays { get; set; }

    public DateTime? ContractDate { get; set; }
    public int? WorkingHours { get; set; }

    public string? IdCardNumber { get; set; }
    public DateTime? IdCardDate { get; set; }
    public string? IdCardAuthority { get; set; }
    public string? Address { get; set; }
}
