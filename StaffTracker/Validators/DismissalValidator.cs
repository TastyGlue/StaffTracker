namespace StaffTracker.Validators;

public partial class DismissalValidator : AbstractValidator<Dismissal>
{
    private readonly ILocalizationService L;

    public DismissalValidator(ILocalizationService l)
    {
        L = l;

        // EntryBase properties
        RuleFor(x => x.EntryDate)
            .NotEmpty()
            .WithMessage(L["Field is required"]);

        RuleFor(x => x.ConsideredFromDate)
            .NotEmpty()
            .WithMessage(L["Field is required"]);

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

        // Dismissal specific properties
        RuleFor(x => x.LabourCodeArticle)
            .NotEmpty()
            .WithMessage(L["Field is required"])
            .GreaterThan(0)
            .WithMessage(L["Value must be greater than 0"]);

        RuleFor(x => x.LabourCodeParagraph)
            .GreaterThan(0)
            .When(x => x.LabourCodeParagraph.HasValue)
            .WithMessage(L["Value must be greater than 0"]);

        RuleFor(x => x.LabourCodeItem)
            .GreaterThan(0)
            .When(x => x.LabourCodeItem.HasValue)
            .WithMessage(L["Value must be greater than 0"]);
    }
}
