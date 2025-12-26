using System.ComponentModel.DataAnnotations.Schema;

namespace Dismissal_Appointment.Models;

public class EntryBase
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public EntryType EntryType { get; set; }
    public DateTime? EntryDate { get; set; } = DateTime.Now.Date;

    public DateTime? ConsideredFromDate { get; set; }
    public bool IsNRAConfirmed { get; set; }

    public string CompanyName { get; set; } = default!;
    public string? Division { get; set; }

    public string IDN { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string? SecondName { get; set; }
    public string Surname { get; set; } = default!;

    [NotMapped]
    public string FullName => $"{FirstName} {(string.IsNullOrWhiteSpace(SecondName) ? "" : SecondName + " ")}{Surname}";
}
