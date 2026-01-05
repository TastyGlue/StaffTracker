using CommunityToolkit.Maui.Storage;

namespace StaffTracker.Components.Layout;

public partial class AppSettingsDialog : ExtendedComponentBase
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Inject] private IFolderPicker FolderPicker { get; set; } = null!;

    private AppSettings? settings;
    private AppSettings? originalSettings;
    private SettingsCategory selectedCategory = SettingsCategory.Language;

    private string SelectedCategoryTitle => selectedCategory switch {
        SettingsCategory.Language => Localizer["Settings_Language"],
        SettingsCategory.FormSettings => Localizer["Settings_FormSettings"],
        SettingsCategory.GridState => Localizer["Settings_GridState"],
        SettingsCategory.ExportSettings => Localizer["Settings_ExportSettings"],
        _ => ""
    };

    protected override async Task OnInitializedAsync()
    {
        var loadedSettings = await AppSettingsService.GetAsync();
        if (loadedSettings != null)
        {
            // Create a copy for editing
            settings = new AppSettings
            {
                Culture = loadedSettings.Culture,
                GridStateSortsSaving = loadedSettings.GridStateSortsSaving,
                GridStateFiltersSaving = loadedSettings.GridStateFiltersSaving,
                GridStatePageSizeSaving = loadedSettings.GridStatePageSizeSaving,
                GridStatePageIndexSaving = loadedSettings.GridStatePageIndexSaving,
                GridStateHiddenColumnsSaving = loadedSettings.GridStateHiddenColumnsSaving,
                FormCreateNew = loadedSettings.FormCreateNew,
                FormFieldEntryDate = loadedSettings.FormFieldEntryDate,
                FormFieldCompany = loadedSettings.FormFieldCompany,
                FormFieldDivision = loadedSettings.FormFieldDivision,
                ExportPreferredDownloadDestination = loadedSettings.ExportPreferredDownloadDestination,
                ExportDefaultFileName = loadedSettings.ExportDefaultFileName
            };

            // Keep original for comparison
            originalSettings = loadedSettings;
        }
    }

    private async Task SelectFolder()
    {
        try
        {
            var result = await FolderPicker.PickAsync(settings.ExportPreferredDownloadDestination, CancellationToken.None);

            if (result.IsSuccessful && result.Folder != null)
            {
                settings.ExportPreferredDownloadDestination = result.Folder.Path;
            }
        }
        catch (Exception ex)
        {
            Notify(Localizer["FileSelectionError"], Severity.Error);
            string errorMessage = Utils.Utils.GetFullExceptionMessage(ex);
            Log.Error(errorMessage);
        }
    }

    private void SelectCategory(SettingsCategory category)
    {
        selectedCategory = category;
    }

    private async Task Save()
    {
        if (settings != null)
        {
            await AppSettingsService.UpdateAsync(settings);

            // Update culture if it changed
            if (settings.Culture != originalSettings?.Culture && settings.Culture != null)
            {
                Localizer.SetCulture(settings.Culture);
            }

            MudDialog.Close(DialogResult.Ok(true));
        }
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }

    private void Close()
    {
        MudDialog.Cancel();
    }

    private enum SettingsCategory
    {
        Language,
        FormSettings,
        GridState,
        ExportSettings
    }
}
