namespace Dismissal_Appointment.Components.Pages.Abstract;

public partial class FormBase<T> : ExtendedComponentBase
    where T : EntryBase, new()
{
    [Parameter] public T Model { get; set; } = new();
    [Parameter] public bool IsCreate { get; set; }
    [Parameter] public EventCallback OnValidSubmit { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    protected void CancelHandler()
    {
        NavManager.NavigateTo("/");
    }

    protected async Task DeleteEntry()
    {
        var parameters = new DialogParameters { ["Entry"] = Model, ["IsCardVisible"] = false };
        var options = new DialogOptions { MaxWidth = MaxWidth.Large, BackdropClick = true, CloseOnEscapeKey = true, Position = DialogPosition.Center, BackgroundClass = "backgroud-dimmed" };
        var dialog = await DialogService.ShowAsync<ConfirmDeleteDialog>(null, parameters, options);

        var result = await dialog.Result;
        if (result is not null && !result.Canceled)
        {
            NavManager.NavigateTo("/");
        }
    }

    protected void ConvertDaysToYearsMonthsDays(int totalDays, out int years, out int months, out int days)
    {
        years = totalDays / 365;
        int remainingDays = totalDays % 365;
        months = remainingDays / 30;
        days = remainingDays % 30;
    }

    protected int? ConvertYearsMonthsDaysToDays(int years, int months, int days)
    {
        int totalDays = (years * 365) + (months * 30) + days;
        return totalDays > 0 ? totalDays : null;
    }
}
