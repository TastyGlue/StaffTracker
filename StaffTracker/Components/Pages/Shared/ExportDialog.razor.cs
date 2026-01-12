using CommunityToolkit.Maui.Storage;
using Microsoft.AspNetCore.Components.Forms;

namespace StaffTracker.Components.Pages.Shared;

public partial class ExportDialog : ExtendedComponentBase
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = default!;
    [Inject] private IFolderPicker FolderPicker { get; set; } = default!;
    [Inject] private AppSettingsService AppSettingsService { get; set; } = default!;
    [Inject] private IExportService ExportService { get; set; } = default!;
    [Inject] private AppDbContext DbContext { get; set; } = default!;

    // Properties
    private AppSettings AppSettings { get; set; } = default!;
    private EditForm Form { get; set; } = default!;
    private ExportForm Model { get; set; } = new()
    {
        Day = DateTime.Today,
        Month = DateTime.Today,
        Year = DateTime.Today,
        RangeStartDate = DateTime.Today.AddDays(-7),
        RangeEndDate = DateTime.Today
    };

    protected override async Task OnInitializedAsync()
    {
        base.OnInitialized();

        AppSettings = await AppSettingsService.GetAsync();

        Model.Folder = AppSettings.ExportPreferredDownloadDestination
            ?? Utils.Utils.GetDefaultDownloadFolder();
        Model.FileName = AppSettings.ExportDefaultFileName;
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
            // Determine export parameters
            string baseFileName = (string.IsNullOrWhiteSpace(Model.FileName)) ? "StaffTracker_Export" : Model.FileName;
            string fileName;
            IQueryable<EntryBase> entriesQuery;

            if (Model.ExportType == ExportType.ExportType_Day)
            {
                fileName = $"{baseFileName}_{Model.Day!.Value:yyyy-MM-dd}.xlsx";

                // Query entries for the selected date
                var selectedDate = Model.Day.Value.Date;
                entriesQuery = DbContext.Entries
                    .Where(e => e.EntryDate.HasValue && e.EntryDate.Value.Date == selectedDate);
            }
            else if (Model.ExportType == ExportType.ExportType_Month)
            {
                fileName = $"{baseFileName}_{Model.Month!.Value:yyyy-MM}.xlsx";

                // Query entries for the selected month
                var selectedMonth = Model.Month.Value.Month;
                entriesQuery = DbContext.Entries
                    .Where(e => e.EntryDate.HasValue && e.EntryDate.Value.Month == selectedMonth);
            }
            else if (Model.ExportType == ExportType.ExportType_Year)
            {
                fileName = $"{baseFileName}_{Model.Year!.Value.Year}.xlsx";

                // Query entries for the selected year
                var selectedYear = Model.Year.Value.Year;
                entriesQuery = DbContext.Entries
                    .Where(e => e.EntryDate.HasValue && e.EntryDate.Value.Year == selectedYear);
            }
            else if (Model.ExportType == ExportType.ExportType_Range)
            {
                fileName = $"{baseFileName}_({Model.RangeStartDate!.Value:yyyy-MM-dd} - {Model.RangeEndDate!.Value:yyyy-MM-dd}).xlsx";

                // Query entries for the selected date
                var startDate = Model.RangeStartDate.Value.Date;
                var endDate = Model.RangeEndDate.Value.Date;
                entriesQuery = DbContext.Entries
                    .Where(e => e.EntryDate.HasValue && e.EntryDate.Value.Date >= startDate && e.EntryDate.Value.Date <= endDate);
            }
            else
            {
                Notify(Localizer["ExportValidationError"], Severity.Warning);
                return;
            }

            // Execute query
            var entries = await entriesQuery.OrderBy(e => e.Id).ToListAsync();

            // Apply Company name filtering
            if (!string.IsNullOrWhiteSpace(Model.Company))
                entries = entries.Where(e => e.CompanyName.Equals(Model.Company, StringComparison.OrdinalIgnoreCase)).ToList();

            // Check if there are entries to export
            if (entries.Count == 0)
            {
                Notify(Localizer["NoEntriesToExport"], Severity.Warning);
                return;
            }

            string filePath = Path.Combine(Model.Folder, fileName);

            // Ensure unique filename (Windows-style renaming if file exists)
            filePath = GetUniqueFilePath(filePath);

            // Generate Excel file
            await ExportService.ExportToExcelAsync(entries, filePath);

            // Open the file with the default application
            await Launcher.Default.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath),
                Title = Localizer["OpenExportedFile"]
            });

            // Success message
            Notify(Localizer["ExportSuccess"], Severity.Success);
            MudDialog.Close(DialogResult.Ok(filePath));
        }
        catch (Exception ex)
        {
            Notify(Localizer["ExportError"], Severity.Error);
            string errorMessage = Utils.Utils.GetFullExceptionMessage(ex);
            Log.Error($"Export failed: {errorMessage}");
        }
    }

    /// <summary>
    /// Generates a unique file path using Windows-style indexing if the file already exists
    /// </summary>
    /// <param name="filePath">The original file path</param>
    /// <returns>A unique file path (e.g., "file (1).xlsx", "file (2).xlsx", etc.)</returns>
    private static string GetUniqueFilePath(string filePath)
    {
        // If file doesn't exist, return the original path
        if (!File.Exists(filePath))
            return filePath;

        // Extract directory, filename without extension, and extension
        string directory = Path.GetDirectoryName(filePath) ?? string.Empty;
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
        string extension = Path.GetExtension(filePath);

        // Try incrementing index until we find a unique filename
        int index = 1;
        string newFilePath;
        do
        {
            string newFileName = $"{fileNameWithoutExtension} ({index}){extension}";
            newFilePath = Path.Combine(directory, newFileName);
            index++;
        }
        while (File.Exists(newFilePath));

        return newFilePath;
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }
}
