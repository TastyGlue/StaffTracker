# Data Export Feature

## Overview
The data export feature allows users to export employee entry data (appointments and dismissals) to Excel format. Users can export data for a specific day or an entire year, and the exported file is automatically opened upon successful creation.

## Implementation Status

### ✅ Completed
- **UI Components**: Export dialog fully designed and implemented
- **Export Type Selection**: Tab-based interface for Day/Year export selection
- **Date/Year Input**: Date picker with mask (dd.MM.yyyy) and year numeric field
- **Folder Selection**: Native Windows folder picker integration using CommunityToolkit.Maui
- **Localization**: All UI strings localized in English and Bulgarian
- **Validation**: Input validation for date/year and folder path
- **Dependency Injection**: Proper service registration in MauiProgram.cs

### ⏳ In Progress / Pending
- **Excel Generation**: Export logic implementation (TODO)
- **Database Queries**: Filtering entries by date/year (TODO)
- **File Operations**: Save and open exported files (TODO)
- **Error Handling**: Comprehensive error handling for file operations (TODO)

## Feature Components

### Enums (`StaffTracker/Enums/`)

#### ExportType.cs
Defines export types for data export functionality:
- `Day` - Export all entries for a specific day
- `Year` - Export all entries for a specific year

### UI Components

#### Export Button (`All.razor`)
- **Location**: Main data grid toolbar (All Entries page)
- **Icon**: Download icon (`Icons.Material.Filled.Download`)
- **Tooltip**: Localized "Export" text
- **Action**: Opens the Export Dialog when clicked
- **Implementation**: Calls `OpenExportDialog()` method in `All.razor.cs:277`

#### ExportDialog.razor
Dialog component that allows users to configure and execute data exports.

**Location**: `StaffTracker/Components/Pages/Shared/ExportDialog.razor`

**Features**:
1. **Export Type Selection**
   - **Tab-based interface** (MudTabs) to choose between Day or Year export
   - Each tab displays the relevant input field
   - Tabs keep their state alive when switching between them
   - Clean, modern UI with no slider indicator

2. **Date/Year Input**
   - **Day Export Tab**: MudDatePicker with dd.MM.yyyy format and mask
     - Editable text field with date mask
     - Calendar picker dialog
     - Auto-close on date selection
     - Required field validation
   - **Year Export Tab**: MudNumericField for year selection
     - Range: 1900-2100
     - Required field validation

3. **Folder Selection**
   - **Text field with folder icon adornment** (clean single-input design)
   - User can manually type/paste folder path
   - Clicking folder icon opens native Windows folder picker
   - Defaults to user's Downloads folder
   - Uses `CommunityToolkit.Maui.Storage.IFolderPicker` for native dialog

4. **Export Actions**
   - **Cancel**: Closes dialog without performing export
   - **Export**: Validates input and performs the export operation (placeholder implementation)

**Dialog Structure**:
- Inherits from `ExtendedComponentBase` for automatic localization support
- Uses MudDialog component with three sections:
  - `TitleContent`: Dialog title with download icon
  - `DialogContent`: Tab-based export type selector with date/year pickers and folder selection (min-width: 400px)
  - `DialogActions`: Cancel and Export buttons
- Custom CSS styling for tab borders and layout

### Code-Behind (`ExportDialog.razor.cs`)

**Location**: `StaffTracker/Components/Pages/Shared/ExportDialog.razor.cs`

**Dependencies**:
- `CommunityToolkit.Maui.Storage.IFolderPicker` - Injected for native folder picker functionality

**Properties**:
- `IMudDialogInstance MudDialog` - Cascading parameter for dialog instance
- `IFolderPicker FolderPicker` - Injected service for folder selection
- `int SelectedTabIndex` - Currently active tab (0 = Day, 1 = Year)
- `ExportType SelectedExportType` - Computed from SelectedTabIndex using switch expression
- `DateTime? SelectedDate` - Selected date for Day export (defaults to today)
- `int? SelectedYear` - Selected year for Year export (defaults to current year)
- `string SelectedFolder` - Path to selected export folder (defaults to Downloads)

**Key Methods**:
- `OnInitialized()` - Initializes default folder to Downloads directory, falls back to MyDocuments if Downloads doesn't exist
- `SelectFolder()` - Opens native Windows folder picker dialog using `IFolderPicker.PickAsync()`, updates `SelectedFolder` on success
- `ValidateInput()` - Validates that date/year is selected and folder path is not empty
- `PerformExport()` - Placeholder method that validates input and prepares file name (TODO: implement Excel generation)
- `Cancel()` - Closes dialog without exporting

## User Workflow

1. **Open Export Dialog**
   - User clicks Export button in main data grid toolbar
   - Dialog opens with default settings (Year export, current year, Downloads folder)

2. **Configure Export**
   - User selects export type:
     - **Day Export**: Exports all entries with EntryDate matching the selected date
     - **Year Export**: Exports all entries with EntryDate in the selected year
   - User selects date or year (depending on export type)
   - User selects destination folder (optional, defaults to Downloads)

3. **Execute Export**
   - User clicks Export button
   - Application validates input (ensures date/year is selected)
   - Application queries database for matching entries
   - Application generates Excel file with entry data
   - Application saves file to selected folder with naming pattern:
     - Day export: `Entries_YYYY-MM-DD.xlsx`
     - Year export: `Entries_YYYY.xlsx`

4. **Post-Export Actions**
   - **Success**:
     - Success notification displayed
     - Excel file is automatically opened in default application
     - Dialog closes
   - **Failure**:
     - Error notification displayed with error details
     - Dialog remains open for user to retry or cancel

## Export File Format

### File Naming Convention
- **Day Export**: `Entries_YYYY-MM-DD.xlsx` (e.g., `Entries_2026-01-15.xlsx`)
- **Year Export**: `Entries_YYYY.xlsx` (e.g., `Entries_2026.xlsx`)

### Excel Structure
The exported Excel file contains the following columns:

| Column | Description | Data Type | Format |
|--------|-------------|-----------|--------|
| Id | Entry ID | Integer | - |
| Company | Company name | String | - |
| Division | Division/Department | String | - |
| Entry Type | Appointment or Dismissal | String | Localized |
| Entry Date | Date of entry | Date | dd.MM.yyyy |
| Considered From Date | Date entry is considered from | Date | dd.MM.yyyy |
| IDN | Identification number | String | - |
| First Name | Employee's first name | String | - |
| Second Name | Employee's middle name | String | - |
| Surname | Employee's surname | String | - |
| Is NRA Confirmed | NRA confirmation status | Boolean | Checkbox/Yes/No |

**Additional Columns for Appointments**:
| Column | Description | Data Type | Format |
|--------|-------------|-----------|--------|
| Position | Job position | String | - |
| Salary | Employee salary | Decimal | Currency |
| Length of Service Days | Days of service | Integer | - |
| ID Card Number | ID card number | String | - |
| ID Card Date | ID card issue date | Date | dd.MM.yyyy |

## Localization Keys

### Required Translation Keys

#### Dialog Elements
- `ExportDialogTitle` - Dialog title ("Export" / "Експорт")
- `ExportType` - Export type label ("Export Type" / "Тип експорт")
- `DayExport` - Day export option ("Day Export" / "Дневен експорт")
- `YearExport` - Year export option ("Year Export" / "Годишен експорт")
- `SelectDate` - Date picker label ("Select Date" / "Изберете дата")
- `SelectYear` - Year picker label ("Select Year" / "Изберете година")
- `ExportFolder` - Folder picker label ("Export Folder" / "Папка за експорт")
- `SelectFolder` - Folder picker button ("Select Folder" / "Избери папка")
- `Export` - Export button ("Export" / "Експорт")
- `Cancel` - Cancel button ("Cancel" / "Отказ")

#### Notifications
- `ExportSuccess` - Success message ("Export created successfully" / "Експортът е създаден успешно")
- `ExportError` - Error message ("An error occurred during export" / "Възникна грешка при експортирането")
- `ExportValidationError` - Validation error ("Please select a date/year" / "Моля изберете дата/година")

#### File Opening
- `FileOpenError` - File open error ("Could not open the exported file" / "Не може да се отвори експортираният файл")

## Implementation Notes

### Database Queries

**Day Export Query**:
```csharp
var entries = await DbContext.Entries
    .Where(e => e.EntryDate.HasValue &&
                e.EntryDate.Value.Date == selectedDate.Date)
    .Include(e => (e as Appointment))  // Include appointment-specific data
    .OrderBy(e => e.Id)
    .ToListAsync();
```

**Year Export Query**:
```csharp
var entries = await DbContext.Entries
    .Where(e => e.EntryDate.HasValue &&
                e.EntryDate.Value.Year == selectedYear)
    .Include(e => (e as Appointment))  // Include appointment-specific data
    .OrderBy(e => e.Id)
    .ToListAsync();
```

### File Operations

**Opening Exported File** (MAUI):
```csharp
await Launcher.Default.OpenAsync(new OpenFileRequest
{
    File = new ReadOnlyFile(filePath),
    Title = "Open Exported File"
});
```

**Folder Picker** (CommunityToolkit.Maui):
```csharp
// Implemented in ExportDialog.razor.cs
var result = await FolderPicker.PickAsync(SelectedFolder, CancellationToken.None);
if (result.IsSuccessful && result.Folder != null)
{
    SelectedFolder = result.Folder.Path;
}
```

**Dependency Injection Setup** (MauiProgram.cs):
```csharp
// Add using statements
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;

// Register in CreateMauiApp()
builder
    .UseMauiApp<App>()
    .UseMauiCommunityToolkit()  // Register Community Toolkit
    .ConfigureFonts(...);

// Register IFolderPicker service
builder.Services.AddSingleton<IFolderPicker>(FolderPicker.Default);
```

### Error Handling

The export feature handles the following error scenarios:
1. **Validation Errors**: User hasn't selected a date/year
2. **Database Errors**: Query execution fails
3. **File System Errors**: Cannot write to selected folder, disk full, permissions
4. **Excel Generation Errors**: Data conversion issues, library errors
5. **File Open Errors**: Cannot open file with default application

All errors are logged and displayed to the user via notifications.

### Future Enhancements

Potential future improvements to the export feature:
1. **Column Selection**: Allow users to choose which columns to export
2. **Export Formats**: Support for PDF, CSV formats
3. **Custom Date Ranges**: Export for custom date ranges (e.g., month, quarter)
4. **Filtering**: Export only filtered data from grid
5. **Styling**: Apply formatting to Excel files (headers, borders, colors)
6. **Templates**: Use pre-designed Excel templates for exports
7. **Email Export**: Email exported file directly from the application
8. **Schedule Exports**: Automated periodic exports

## Technical Dependencies

### NuGet Packages

#### Installed
- **CommunityToolkit.Maui** (v9.1.1) - MAUI Community Toolkit
  - Provides cross-platform UI components and helpers
  - Includes `IFolderPicker` service for native folder selection
  - Registered in `MauiProgram.cs` with `.UseMauiCommunityToolkit()`

#### To Be Added
- **EPPlus** (v7.x) or **ClosedXML** (v0.102.x) - Excel file generation
  - EPPlus: More feature-rich, requires license for commercial use
  - ClosedXML: Open-source, MIT license, good for basic Excel operations

### MAUI APIs Used
- `CommunityToolkit.Maui.Storage.IFolderPicker` - Native folder selection dialog (Windows)
- `Microsoft.Maui.ApplicationModel.Launcher` - Opening files with default application (to be implemented)

## Testing Scenarios

### Manual Testing Checklist

#### UI & Interaction (Completed ✅)
- [x] Export button opens dialog correctly
- [x] Dialog displays with default values (Year export, current year, Downloads folder)
- [x] Can switch between Day and Year export types using tabs
- [x] Date picker appears for Day export with dd.MM.yyyy format
- [x] Year picker appears for Year export (numeric field, range 1900-2100)
- [x] Folder text field is editable (manual path entry)
- [x] Folder icon adornment opens native Windows folder picker
- [x] Folder picker starts at current selected folder
- [x] Selected folder updates text field after picker selection
- [x] Validation prevents export without date/year selection
- [x] Cancel button closes dialog without exporting
- [x] Localization works for both Bulgarian and English
- [x] Tab state persists when switching between tabs

#### Export Functionality (Pending ⏳)
- [ ] Day export creates file with correct entries
- [ ] Year export creates file with correct entries
- [ ] File naming follows convention (Entries_YYYY-MM-DD.xlsx / Entries_YYYY.xlsx)
- [ ] Excel file contains all expected columns
- [ ] Excel file contains correct data
- [ ] Appointment-specific columns appear for appointment entries
- [ ] File opens automatically after successful export
- [ ] Success notification displays on successful export
- [ ] Error notification displays on failed export
- [ ] Dialog closes after successful export

### Edge Cases
- [ ] Export when no entries exist for selected date/year (creates empty Excel file)
- [ ] Export with very large datasets (10,000+ entries)
- [ ] Export to folder with insufficient permissions
- [ ] Export to full disk
- [ ] Export with special characters in data (Unicode, emojis)
- [ ] Export when file with same name already exists (overwrite behavior)
