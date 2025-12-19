using Microsoft.AspNetCore.Components;

namespace Dismissal_Appointment.Components.Pages.Abstract;

public partial class ExtendedComponentBase : ComponentBase, IDisposable
{
    [Inject] protected ILocalizationService Localizer { get; set; } = null!;

    [Inject] protected IPageTitleService PageTitleService { get; set; } = null!;

    protected override void OnInitialized()
    {
        Localizer.OnCultureChanged += StateHasChanged;
        PageTitleService.OnChange += StateHasChanged;
    }

    protected void SetTitle(string title)
    {
        PageTitleService.SetTitle(title);
    }

    public void Dispose()
    {
        Localizer.OnCultureChanged -= StateHasChanged;
        PageTitleService.OnChange -= StateHasChanged;
        GC.SuppressFinalize(this);
    }
}
