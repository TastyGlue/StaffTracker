namespace StaffTracker.Services;

/// <summary>
/// Service for exporting employee entry data to various formats
/// </summary>
public interface IExportService
{
    /// <summary>
    /// Exports entries to an Excel file with Bulgarian formatting
    /// </summary>
    /// <param name="entries">List of entries to export</param>
    /// <param name="filePath">Full path where the Excel file should be saved</param>
    /// <returns>Task representing the async operation</returns>
    Task ExportToExcelAsync(IEnumerable<EntryBase> entries, string filePath);
}
