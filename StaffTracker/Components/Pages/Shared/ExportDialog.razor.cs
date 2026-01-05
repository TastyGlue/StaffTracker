using CommunityToolkit.Maui.Storage;
using Microsoft.AspNetCore.Components.Forms;

namespace StaffTracker.Components.Pages.Shared;

public partial class ExportDialog : ExtendedComponentBase
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = default!;
    [Inject] private IFolderPicker FolderPicker { get; set; } = default!;
    [Inject] private AppSettingsService AppSettingsService { get; set; } = default!;

    // Properties
    private AppSettings AppSettings { get; set; } = default!;
    private EditForm Form { get; set; } = default!;
    private ExportForm Model { get; set; } = new()
    {
        Day = DateTime.Today,
        Year = DateTime.Today
    };
    private int SelectedTabIndex { get; set; }
    private string SelectedFolder { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        base.OnInitialized();

        AppSettings = await AppSettingsService.GetAsync();

        Model.Folder = AppSettings.ExportPreferredDownloadDestination
            ?? Utils.Utils.GetDefaultDownloadFolder();
        Model.FileName = AppSettings.ExportDefaultFileName ?? default!;
    }

    private async Task SelectFolder()
    {
        try
        {
            var result = await FolderPicker.PickAsync(Model.Folder, CancellationToken.None);

            if (result.IsSuccessful && result.Folder != null)
            {
                Model.Folder = result.Folder.Path;
            }
        }
        catch (Exception ex)
        {
            Notify(Localizer["FileSelectionError"], Severity.Error);
            string errorMessage = Utils.Utils.GetFullExceptionMessage(ex);
            Log.Error(errorMessage);
        }
    }

    private async Task PerformExport()
    {
        var isValid = Form.EditContext!.Validate();
        if (!isValid)
            return;

        try
        {
            // TODO: Implement actual export logic here
            // For now, just show a success message and close

            // Determine export parameters
            string fileName;
            if (Model.ExportType == ExportType.Day)
            {
                fileName = $"{Model.FileName}_{Model.Day!.Value:yyyy-MM-dd}.xlsx";
                // TODO: Query entries for the selected date
            }
            else if (Model.ExportType == ExportType.Year)
            {
                fileName = $"{Model.FileName}_{Model.Year!.Value.Year}.xlsx";
                // TODO: Query entries for the selected year
            }
            else
            {
                Notify(Localizer["ExportValidationError"], Severity.Warning);
                return;
            }

            string filePath = Path.Combine(Model.Folder, fileName);

            // TODO: Generate Excel file
            // TODO: Save to filePath
            // TODO: Open the file

            // Placeholder success message
            Notify(Localizer["ExportSuccess"], Severity.Success);
            MudDialog.Close(DialogResult.Ok(filePath));
        }
        catch (Exception ex)
        {
            Notify(Localizer["ExportError"], Severity.Error);
            string errorMessage = Utils.Utils.GetFullExceptionMessage(ex);
            Log.Error(errorMessage);
        }
    }

    private void TabIndexChanged(int value)
    {
        SelectedTabIndex = value;
        Model.ExportType = SelectedTabIndex switch
        {
            0 => ExportType.Day,
            1 => ExportType.Year,
            _ => ExportType.Day
        };
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }
}
