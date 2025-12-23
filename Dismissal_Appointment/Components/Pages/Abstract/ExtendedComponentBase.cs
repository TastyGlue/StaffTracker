namespace Dismissal_Appointment.Components.Pages.Abstract;

public partial class ExtendedComponentBase : ComponentBase, IDisposable
{
    [Inject] protected NavigationManager NavManager { get; set; } = null!;
    [Inject] protected ILocalizationService Localizer { get; set; } = null!;
    [Inject] protected IPageTitleService PageTitleService { get; set; } = null!;
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;

    protected override void OnInitialized()
    {
        Localizer.OnCultureChanged += StateHasChanged;
        PageTitleService.OnChange += StateHasChanged;
    }

    protected void SetTitle(string title)
    {
        PageTitleService.SetTitle(title);
    }

    protected void Notify(string message, Severity severity, int duration = 5000)
    {
        Snackbar.Add(Localizer[message], severity, config => { config.VisibleStateDuration = duration; });
    }

    public void Dispose()
    {
        Localizer.OnCultureChanged -= StateHasChanged;
        PageTitleService.OnChange -= StateHasChanged;
        GC.SuppressFinalize(this);
    }
}
