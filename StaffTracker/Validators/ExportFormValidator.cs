namespace StaffTracker.Validators;

public partial class ExportFormValidator : AbstractValidator<ExportForm>
{
    private readonly ILocalizationService L;

    public ExportFormValidator(ILocalizationService l)
    {
        L = l;

        RuleFor(x => x.Day)
            .NotEmpty()
            .When(x => x.ExportType == ExportType.ExportType_Day)
            .WithMessage(L["Field is required"]);

        RuleFor(x => x.Month)
            .NotEmpty()
            .When(x => x.ExportType == ExportType.ExportType_Month)
            .WithMessage(L["Field is required"]);

        RuleFor(x => x.Year)
            .NotEmpty()
            .When(x => x.ExportType == ExportType.ExportType_Year)
            .WithMessage(L["Field is required"]);

        RuleFor(x => x.RangeStartDate)
            .NotEmpty()
            .When(x => x.ExportType == ExportType.ExportType_Range)
            .WithMessage(L["Field is required"])
            .LessThanOrEqualTo(x => x.RangeEndDate)
            .When(x => x.ExportType == ExportType.ExportType_Range)
            .WithMessage(L["Start Date must be equal or less than End Date"]);

        RuleFor(x => x.RangeEndDate)
            .NotEmpty()
            .When(x => x.ExportType == ExportType.ExportType_Range)
            .WithMessage(L["Field is required"]);

        RuleFor(x => x.Folder)
            .NotEmpty()
            .WithMessage(L["Field is required"]);
    }
}
