namespace StaffTracker.Validators;

public partial class ExportFormValidator : AbstractValidator<ExportForm>
{
    private readonly ILocalizationService L;

    public ExportFormValidator(ILocalizationService l)
    {
        L = l;

        RuleFor(x => x.Day)
            .NotEmpty()
            .When(x => x.ExportType == ExportType.Day)
            .WithMessage(L["Field is required"]);

        RuleFor(x => x.Year)
            .NotEmpty()
            .When(x => x.ExportType == ExportType.Year)
            .WithMessage(L["Field is required"]);

        RuleFor(x => x.Folder)
            .NotEmpty()
            .WithMessage(L["Field is required"]);

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage(L["Field is required"]);
    }
}
