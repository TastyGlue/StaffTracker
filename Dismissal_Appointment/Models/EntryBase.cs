namespace Dismissal_Appointment.Models;

public abstract class EntryBase
{
    public EntryType EntryType { get; set; }
    public DateTime Date { get; set; }
    public bool IsNRAConfirmed { get; set; }

    public string CompanyName { get; set; } = default!;
    public string IDN { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string? SecondName { get; set; }
    public string Surname { get; set; } = default!;
    public string LabourCodeArticle { get; set; } = default!;
}
