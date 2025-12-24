namespace Dismissal_Appointment.Validators;

public partial class AppointmentValidator : AbstractValidator<Appointment>
{
    private readonly ILocalizationService L;
    public AppointmentValidator(ILocalizationService l)
    {
        L = l;

        // EntryBase properties
        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .WithMessage(L["Field is required"]);

        RuleFor(x => x.IDN)
            .NotEmpty()
            .WithMessage(L["Field is required"])
            .Length(10)
            .WithMessage(L["IDN must be 10 digits"])
            .Matches(@"^\d{10}$")
            .WithMessage(L["IDN must contain only digits"]);

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage(L["Field is required"]);

        RuleFor(x => x.Surname)
            .NotEmpty()
            .WithMessage(L["Field is required"]);

        // Appointment-specific properties
        RuleFor(x => x.Salary)
            .GreaterThan(0)
            .WithMessage(L["Salary must be positive"]);

        RuleFor(x => x.Position)
            .NotEmpty()
            .WithMessage(L["Field is required"]);

        RuleFor(x => x.WorkExperienceDays)
            .GreaterThanOrEqualTo(0)
            .When(x => x.WorkExperienceDays.HasValue)
            .WithMessage(L["Work experience cannot be negative"]);

        RuleFor(x => x.WorkExperienceInProfessionDays)
            .GreaterThanOrEqualTo(0)
            .When(x => x.WorkExperienceInProfessionDays.HasValue)
            .WithMessage(L["Work experience in profession cannot be negative"]);

        RuleFor(x => x.WorkingHours)
            .GreaterThan(0)
            .When(x => x.WorkingHours.HasValue)
            .WithMessage(L["Working hours must be positive"]);

        RuleFor(x => x.IdCardNumber)
            .Matches(@"^\d{9}$")
            .When(x => !string.IsNullOrWhiteSpace(x.IdCardNumber))
            .WithMessage(L["ID card number must be 9 digits"]);
    }
}
