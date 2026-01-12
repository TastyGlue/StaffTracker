# StaffTracker - Employee Accounting Application

## Project Overview
A simple CRUD accounting application built with .NET MAUI Blazor Hybrid, running exclusively on Windows. The application tracks appointments (hiring) and dismissals of employees for accounting purposes.

## Technology Stack
- **Framework**: .NET MAUI Blazor Hybrid
- **Platform**: Windows only
- **Language**: C#
- **Database**: SQLite with Entity Framework Core 9.0
- **Data Storage**: Local file in user's AppData\Local folder

## Project Structure

### Models (`StaffTracker/Models/`)

#### EntryBase.cs
Abstract base class for all employee entries containing common properties:
- `EntryType` - Enum indicating if entry is Appointment or Dismissal
- `Date` - Date of the appointment/dismissal
- `IsNRAConfirmed` - NRA (National Revenue Agency) confirmation status
- `CompanyName` - Name of the company
- `IDN` - Identification number
- `FirstName` - Employee's first name
- `SecondName` - Employee's middle name (optional)
- `Surname` - Employee's last name
- `LabourCodeArticle` - Reference to labour code article

#### Appointment.cs
Inherits from EntryBase. Represents employee hiring with additional properties:
- `Salary` - Employee salary (decimal)
- `Position` - Job position
- `LengthOfServiceDays` - Length of service in days (optional)
- `IdCardNumber` - ID card number (optional)
- `IdCardDate` - ID card issue date (optional)

#### Dismissal.cs
Inherits from EntryBase. Represents employee dismissal with no additional properties beyond the base class.

#### Grid State Models (`Models/GridState/`)
Models for persisting MudDataGrid state across navigation and app restarts:
- **`ColumnSortState.cs`** - Stores sort information (PropertyName, Direction, Index)
- **`ColumnFilterState.cs`** - Stores filter criteria (PropertyName, Operator, Value with custom JSON converter)
- **`EntryGridState.cs`** - Main state container holding sorts, filters, page size/index, and hidden columns
- Uses `PrimitiveValueConverter` for dynamic filter value type serialization (string, int, DateTime, bool, decimal, etc.)

#### AppSettings.cs
Model for per-user application settings stored in JSON file in local AppData:
- `Id` (int) - Legacy field, always 1
- `Culture` (string?) - Selected UI culture (e.g., "bg-BG", "en-US")
- **Grid State Settings** (boolean flags):
  - `GridStateSortsSaving` - Save grid sorting state
  - `GridStateFiltersSaving` - Save grid filters state
  - `GridStatePageSizeSaving` - Save page size
  - `GridStatePageIndexSaving` - Save page index
  - `GridStateHiddenColumnsSaving` - Save hidden columns
- **Form Settings** (boolean flags):
  - `FormCreateNew` - Auto-create new form after save
  - `FormFieldEntryDate` - Remember entry date field
  - `FormFieldCompany` - Remember company field
  - `FormFieldDivision` - Remember division field
- **Export Settings**:
  - `ExportPreferredDownloadDestination` (string?) - Default export folder path
  - `ExportDefaultFileName` (string?) - Default base filename for exports
- **Storage**: `%LOCALAPPDATA%\StaffTracker\app_settings.json` (per-user)

#### ExportForm.cs
Model for the export dialog form data:
- `ExportType` (ExportType enum) - Type of export (Day or Year)
- `Day` (DateTime?) - Selected date for Day export
- `Year` (DateTime?) - Selected year for Year export
- `Folder` (string) - Destination folder path for export file
- `FileName` (string) - Base filename for export file (without date suffix)
- **Validation**: Uses FluentValidation with `ExportFormValidator`

### Base Components (`StaffTracker/Components/Pages/Abstract/`)

#### ExtendedComponentBase.cs
Abstract base class for all page components that provides common functionality:
- Inherits from `ComponentBase` and implements `IDisposable`
- Automatically injects `ILocalizationService` (as `Localizer` property)
- Automatically injects `IPageTitleService` (as `PageTitleService` property)
- Automatically subscribes to culture change and page title change events in `OnInitialized()`
- Provides `SetTitle(string title)` helper method for setting page titles
- Automatically handles cleanup and unsubscribes from events in `Dispose()`
- **IMPORTANT**: When overriding `OnInitialized()` or other lifecycle methods, always call `base.OnInitialized()` first to ensure event subscriptions work correctly

### Enums (`StaffTracker/Enums/`)

#### EntryType.cs
Defines entry types:
- `Dismissal = 1`
- `Appointment = 2`

#### ExportType.cs
Defines export types for data export functionality:
- `Day` - Export all entries for a specific day
- `Year` - Export all entries for a specific year

### Data (`StaffTracker/Data/`)

#### AppDbContext.cs
Entity Framework Core DbContext for the application:
- `DbSet<EntryBase> Entries` - Collection of all entry records (base type)
- `DbSet<Appointment> Appointments` - Collection of appointment records
- `DbSet<Dismissal> Dismissals` - Collection of dismissal records
- Uses Table-Per-Type (TPT) mapping strategy for inheritance hierarchy
- Configures decimal precision for salary fields
- Enforces required properties and constraints

#### DatabaseConfig.cs
Configuration helper for database connection:
- `DatabasePath` - Returns the full path to the SQLite database file
- `ConnectionString` - Provides the SQLite connection string
- Database location: `{AppContext.BaseDirectory}\Database\dismissal_appointment.db` (shared across all network users)

#### DatabaseInitializer.cs
Service for database initialization:
- Ensures database is created on application startup
- Called synchronously in `App.xaml.cs` constructor before any pages are created
- Uses `EnsureCreatedAsync()` to create database schema
- Seeds test data (5 appointments and 5 dismissals) on first run
- Registered as transient service in dependency injection

### Services (`StaffTracker/Services/`)

#### LocalizationService.cs
Service for application localization and internationalization:
- `ILocalizationService` - Interface for localization service
- `LocalizationService` - Implementation providing string localization
- Indexer `this[string key]` - Returns localized string for a given key
- Indexer `this[string key, params object[] arguments]` - Returns formatted localized string with arguments
- `CurrentCulture` - Property exposing the current UI culture
- `SetCulture(string culture)` - Method to change the application culture and notify all subscribers
- `OnCultureChanged` - Event that fires when culture is changed, allowing components to re-render with new translations
- Uses resource files from `Resources/Translations/` folder
- Registered as singleton in dependency injection container

#### PageTitleService.cs
Service for managing the page title displayed in MainLayout:
- `IPageTitleService` - Interface for page title service
- `PageTitleService` - Implementation for managing page titles
- `TitleKey` - Property containing the resource key for the current page title
- `OnChange` - Event that fires when the title changes, triggering MainLayout to re-render
- `SetTitle(string titleKey)` - Method to set the page title using a localization resource key
- The title is automatically localized using the LocalizationService
- Registered as scoped service in dependency injection container

#### EntryGridStateService.cs
Service for persisting MudDataGrid state (sorting, filtering, paging, hidden columns) across navigation and app restarts:

**Features:**
- Thread-safe file operations using dual `SemaphoreSlim` locks (`_fileLock` and `_initLock`)
- Lazy initialization pattern to avoid deadlocks (no blocking in constructor)
- JSON persistence to `%LOCALAPPDATA%\StaffTracker\entry_grid_state.json` (per-user)
- Camel case JSON naming policy for readability
- Automatic state saving and loading

**Storage:**
- Persists grid state to JSON file in user's local AppData (per-user preferences)
- Uses custom `PrimitiveValueConverter` to handle dynamic filter value types

**Public API:**
```csharp
EntryGridState GridState { get; }  // Synchronous property (may return default state)
Task<EntryGridState> GetGridStateAsync()  // Safe async getter with guaranteed initialization
Task UpdateFullStateAsync(EntryGridState state)  // Updates and saves complete grid state
```

**Implementation in All.razor.cs:**
- **Hybrid save approach**: Combines timer-based auto-save (every 2 seconds) with navigation event handling
- **LoadGridStateAsync()**: Restores sorts, filters, paging, and hidden columns on component initialization
- **SaveGridStateAsync()**: Captures current grid state and persists to file
- **CheckAndSaveState()**: Detects changes via count-based comparison (sort count, filter count, page size/index, hidden columns)
- **Auto-save timer**: Runs every 2 seconds to detect and save changes
- **Navigation event**: Saves state asynchronously when navigating away (using `async void` event handler)
- **Dispose**: Saves final state using `Task.Run()` to avoid UI thread deadlock

**Special Handling:**
- EntryType enum filter requires integer-to-enum conversion during load
- Uses expression trees (`Utils.CreatePropertySelector()`) to rebuild sort functions from property names
- Registered as singleton in dependency injection container

#### AppSettingsService.cs
Service for managing per-user application settings stored in JSON file:

**Features:**
- Thread-safe file operations using dual `SemaphoreSlim` locks (`_fileLock` and `_initLock`)
- Lazy initialization pattern to avoid deadlocks (no blocking in constructor)
- JSON persistence to `%LOCALAPPDATA%\StaffTracker\app_settings.json` (per-user)
- Camel case JSON naming policy for readability
- Automatic default settings creation on first run
- Registered as **singleton** service in dependency injection container

**Storage:**
- Persists settings to JSON file in user's local AppData (per-user preferences)
- Each network user has their own settings file on their local machine

**Public API:**
```csharp
Task<AppSettings> GetAsync()  // Retrieves current settings (auto-initializes with defaults if needed)
Task UpdateAsync(AppSettings settings)  // Updates and persists settings to JSON file
```

**Default Settings:**
- Culture: "bg-BG"
- All grid state saving flags: true
- All form field remembering flags: false

**Usage:**
Settings control various application behaviors:
- Language/culture selection (per-user)
- Grid state persistence options (sorting, filtering, paging, hidden columns)
- Form behavior (auto-create new, remember field values)

#### ExcelExportService.cs
Service for exporting employee entry data to Excel files with Bulgarian formatting:

**Interface:** `IExportService`
**Implementation:** `ExcelExportService`

**Features:**
- Exports entries to Excel (.xlsx) files using EPPlus library
- Bulgarian language formatting for all labels and values
- Custom form-like layout (not a traditional table structure)
- Separate formatting for Appointment and Dismissal entries
- Automatic column width adjustment with minimum widths for merged cells
- Bold text for key fields (company name, full name, salary, etc.)
- Bordered cells for clear visual separation

**Public API:**
```csharp
Task ExportToExcelAsync(IEnumerable<EntryBase> entries, string filePath);
```

**EPPlus Configuration:**
- Uses non-commercial license: `ExcelPackage.License.SetNonCommercialOrganization("Sitikom 2007")`
- License context must be set before creating any ExcelPackage instances

**Appointment Export Format:**
Each appointment entry is formatted as a custom form with the following structure:
- Row 1: Red bold header "За назначаване"
- Row 2: Company name (bold) | Division
- Row 3: IDN (bold) | Full name (bold)
- Row 4: Salary with currency (bold) | Position with label (bold) | Work experience (merged 3 columns)
- Row 5: Work experience in profession (merged 3 columns, spans row 4-5 area)
- Row 6: Entry date with label (bold) | Contract date with label | Working hours (merged 3 columns)
- Row 7: ID card number with label | ID card date with label | ID card authority (merged 3 columns)
- Row 8: Address with label (merged 5 columns)

**Dismissal Export Format:**
Each dismissal entry is formatted as:
- Row 1: Red bold header "За уволнение"
- Row 2: Company name (bold) | Division
- Row 3: IDN (bold) | Full name (bold)
- Row 4: Entry date with label (bold)
- Row 5: Labour code article with label | Compensation days with label (merged 2 columns)
- Row 6: Garnishment status with label | Leave last month with label (merged 2 columns)

**Field Formatting Rules:**
- **Display value only** (bold): Company name, IDN, Full name
- **Label + value format**: All other fields (e.g., "ЗАПЛАТА - 1400 EUR", "ДЛЪЖНОСТ - Manager")
- **NULL values**: Displayed as "NULL" for missing optional fields
- **Dates**: Formatted as "dd.MM.yyyy" or "NULL"
- **Work experience**: Converted from days to "X години / Y месеци / Z дни" format
- **Labour code article**: Formatted as "Чл.X, п.Y, т.Z" (Article X, paragraph Y, item Z)
- **Garnishment**: Displayed as "ДА" (yes) or "НЕ" (no)

**Column Width Handling:**
- After AutoFit, ensures minimum column width of 20 units
- Prevents merged cells from being cut off due to AutoFit limitations
- Adjustable minimum width for different text densities

**Registered as:** Scoped service in dependency injection container

**Dependencies:**
- EPPlus library for Excel file generation
- Requires `using OfficeOpenXml;` and `using OfficeOpenXml.Style;`

### Resources (`StaffTracker/Resources/Translations/`)

#### SharedResource.resx
Default resource file containing translatable strings for the application:
- Base English language resource file
- Auto-generated Designer.cs provides strongly-typed access
- Configured as embedded resource with PublicResXFileCodeGenerator

#### SharedResource.bg-BG.resx
Bulgarian language resource file:
- Culture-specific translations for Bulgarian (bg-BG)
- Falls back to default resource if translation is missing
- Example translations include UI strings like "Hello, world!" -> "Здравей, свят!"

## Localization

### Configuration
The application is configured with Bulgarian (bg-BG) as the default culture:
- Default culture set in `MauiProgram.cs` on application startup
- `CultureInfo.DefaultThreadCurrentCulture` and `CultureInfo.DefaultThreadCurrentUICulture` both set to "bg-BG"
- Localization service registered in dependency injection with resources path "Resources/Translations"
- Uses .NET resource file (.resx) system for managing translations

### Usage
To use localization in Blazor components:
1. Inject `ILocalizationService` into the component
2. Access localized strings using the indexer: `Localizer["key"]`
3. For formatted strings with parameters: `Localizer["key", arg1, arg2]`
4. Access current culture via `Localizer.CurrentCulture`
5. Subscribe to `OnCultureChanged` event and call `StateHasChanged()` to update when culture switches
6. Implement `IDisposable` and unsubscribe in `Dispose()` method

### Adding New Translations
1. Add the default English string to `SharedResource.resx`
2. Add culture-specific translations to corresponding .resx files (e.g., `SharedResource.bg-BG.resx` for Bulgarian)
3. The LocalizationService will automatically return the key if no translation is found

## Database Setup

### Storage Location
The SQLite database is stored in the application base directory (shared across all network users):
```
{AppContext.BaseDirectory}\Database\dismissal_appointment.db
```

**Network Deployment:**
- When deployed to a network share (e.g., `\\server\apps\DismissalApp\`), the database resides on the server
- All users accessing the application through shortcuts share the same database
- Only one user should access the application at a time (no concurrent access)

### Configuration
- Database connection is configured in `MauiProgram.cs`
- AppDbContext is registered with dependency injection
- Database is automatically initialized when the application starts
- Uses Table-Per-Type mapping for Appointment and Dismissal entities

### Tables
The database contains the following tables:
- **Entries** (base table) - Common employee entry data
- **Appointments** - Appointment-specific data (extends Entries)
- **Dismissals** - Dismissal-specific data (extends Entries)

### NuGet Packages
- `Microsoft.EntityFrameworkCore.Sqlite` (v9.0.11)
- `Microsoft.EntityFrameworkCore.Tools` (v9.0.11)
- `EPPlus` (v7.x) - Excel file generation for export functionality
- `CommunityToolkit.Maui` (v9.1.1) - Cross-platform UI components and helpers (includes IFolderPicker)

## Application Startup

### Initialization Sequence
The application follows a specific initialization order to ensure the database and settings are ready before any Blazor pages load:

1. **`App.xaml.cs` Constructor** (`StaffTracker/App.xaml.cs`)
   - Called when the MAUI application starts
   - Initializes database **synchronously** using `InitializeDatabaseAsync().GetAwaiter().GetResult()`
   - Ensures database creation and seeding complete before proceeding
   - Initializes localization from saved `AppSettings` JSON file (per-user)
   - Uses scoped service provider for database initialization to properly resolve scoped `AppDbContext`

2. **Database Initialization** (via `DatabaseInitializer`)
   - Creates database schema if it doesn't exist in `{AppContext.BaseDirectory}\Database\`
   - Seeds test data (5 appointments, 5 dismissals) on first run
   - Database is shared across all network users

3. **Localization Initialization**
   - Loads saved culture from `AppSettingsService` (JSON file in user's local AppData)
   - Creates default settings JSON file if it doesn't exist (culture: "bg-BG")
   - Sets `DefaultThreadCurrentCulture` and `DefaultThreadCurrentUICulture`
   - Each network user has their own settings file on their local machine

4. **MainPage Creation** (`StaffTracker/MainPage.xaml.cs`)
   - Simple ContentPage with BlazorWebView
   - No initialization logic (handled by App.xaml.cs)
   - Database is guaranteed to exist at this point

5. **Blazor UI Loads**
   - Home page (All.razor.cs) can safely query database
   - Grid state is restored from JSON file in user's local AppData (per-user preferences)
   - Application is ready for user interaction

### Critical Design Notes
- **Synchronous initialization**: Using `.GetAwaiter().GetResult()` in App constructor ensures database exists before any UI loads
- **Race condition prevention**: Previous async `Task.Run()` approach in MainPage caused "no such table" errors
- **Service scoping**: Database initialization uses `CreateScope()` to properly resolve scoped services (`AppDbContext`) from the singleton App instance
- **Data separation**: Employee data (database) is shared on network server, while UI preferences (JSON files) are per-user on local machines

## Purpose
This application serves as an accounting tool to maintain records of:
- Employee appointments (new hires) with detailed information including salary and position
- Employee dismissals with labour code article references
- NRA confirmation tracking for regulatory compliance

## UI/UX

### MudBlazor Components
The application uses MudBlazor for the user interface:
- MudBlazor theme provider configured in `MainLayout.razor`
- Dialog, Popover, and Snackbar providers enabled globally

### Layout Structure
The application uses a simple, clean layout defined in `MainLayout.razor`:
- **Topbar**: Subtle topbar with light background (`#f5f5f5`) and minimal shadow
  - Settings icon (⚙️) in the top-right corner opens the settings dialog
- **Page Title Section**: Centered horizontally below the topbar
  - Displays the current page title with clear separation from content
- **Page Content**: Main content area below the title section
  - Max-width of 1400px, centered horizontally
  - Light background color (`#fafafa`)

### Application Settings Dialog
The settings dialog (`AppSettingsDialog.razor`) provides a centralized interface for customizing application behavior:

**Layout:**
- **Left Panel**: Vertical navigation menu with categories (Language, Form Settings, Grid State)
- **Right Panel**: Settings controls for the selected category
- **Actions**: Cancel, Save, and Close (X) buttons

**Settings Categories:**
1. **Language** (🌐) - UI language selection (Bulgarian/English)
   - Changes take effect immediately after saving
   - Updates both `DefaultThreadCurrentCulture` and `DefaultThreadCurrentUICulture`

2. **Form Settings** (✏️) - Form behavior preferences
   - Create new after save - Auto-opens blank form after successful save
   - Remember Entry Date - Persists entry date across form sessions
   - Remember Company - Persists company name across form sessions
   - Remember Division - Persists division across form sessions

3. **Grid State** (⊞) - Grid state persistence options
   - Save grid sorting - Remembers column sort order
   - Save grid filters - Remembers active filters
   - Save page size - Remembers rows per page
   - Save page index - Remembers current page
   - Save hidden columns - Remembers hidden column state

**Accessing Settings Programmatically:**
```csharp
@inject AppSettingsService AppSettingsService

var settings = await AppSettingsService.GetAsync();
if (settings != null)
{
    // Use settings
    var shouldCreateNew = settings.FormCreateNew;
    var saveFilters = settings.GridStateFiltersSaving;
}
```

### Export Dialog
The export dialog (`ExportDialog.razor`) provides an interface for exporting employee entry data to Excel files:

**Location:** `StaffTracker/Components/Pages/Shared/ExportDialog.razor`

**Access:** Export button in the All Entries page toolbar (download icon)

**Features:**
1. **Export Type Selection**
   - Tab-based interface (Day Export / Year Export)
   - Day Export: Exports all entries for a selected date
   - Year Export: Exports all entries for a selected year

2. **Date/Year Selection**
   - Day Export: MudDatePicker with dd.MM.yyyy format and mask
   - Year Export: MudNumericField (range: 1900-2100)
   - Both fields are required

3. **Folder Selection**
   - Text field for manual path entry or paste
   - Folder icon button opens native Windows folder picker
   - Defaults to user's Downloads folder (or Documents if Downloads doesn't exist)

4. **File Naming**
   - Uses configurable default filename from AppSettings
   - Day export: `{filename}_YYYY-MM-DD.xlsx` (e.g., `Entries_2026-01-07.xlsx`)
   - Year export: `{filename}_YYYY.xlsx` (e.g., `Entries_2026.xlsx`)
   - **Windows-style duplicate handling**: If file exists, appends index (e.g., `Entries_2026-01-07 (1).xlsx`, `Entries_2026-01-07 (2).xlsx`)

5. **Export Actions**
   - **Cancel**: Closes dialog without exporting
   - **Export**: Validates input, queries database, generates Excel file, and opens it

**Export Workflow:**
1. User clicks Export button on All Entries page
2. Dialog opens with default settings (Year export, current year, Downloads folder)
3. User selects export type (Day or Year)
4. User selects date or year
5. User selects destination folder (optional, defaults pre-filled)
6. User clicks Export button
7. Application validates input
8. Application queries database for matching entries
9. If no entries found, displays warning message
10. If entries found, generates Excel file using ExcelExportService
11. File is automatically opened with default Excel application
12. Success notification displayed
13. Dialog closes

**Validation:**
- Date/Year selection is required
- Folder path must not be empty
- Displays warning if no entries found for selected period

**Error Handling:**
- Folder selection errors: Displays error notification and logs exception
- Export errors: Displays error notification, logs exception, keeps dialog open for retry
- File open errors: Handled by MAUI Launcher API

**Dependencies:**
- `IFolderPicker` - Native Windows folder picker (CommunityToolkit.Maui)
- `IExportService` - Excel file generation service
- `AppDbContext` - Database access for querying entries
- `AppSettingsService` - Access to default filename and folder preferences

**Code-Behind:** `ExportDialog.razor.cs`
- Inherits from `ExtendedComponentBase` for automatic localization
- Uses FluentValidation for form validation (ExportFormValidator)
- Implements Windows-style file naming with `GetUniqueFilePath()` helper method

**Localization Keys:**
- `ExportDialogTitle` - Dialog title
- `DayExport` / `YearExport` - Export type options
- `SelectDate` / `SelectYear` - Date/year picker labels
- `ExportFolder` - Folder selection label
- `Export` / `Cancel` - Action buttons
- `ExportSuccess` / `ExportError` / `ExportValidationError` - Notification messages
- `NoEntriesToExport` - Warning when no entries found
- `OpenExportedFile` - File open request title

### Styling Guidelines
- All custom styling for the layout is defined in `wwwroot/css/app.css`
- Layout uses the following CSS classes:
  - `.app-topbar` - Styling for the top application bar
  - `.main-content` - Main content area background
  - `.page-title-container` - Container for centered page title
  - `.page-title` - Page title text styling
  - `.page-content` - Content area with padding and max-width
  - `.app-settings-dialog` - Main settings dialog container
  - `.settings-container` - Flexbox layout for two-panel design
  - `.settings-nav` - Left navigation panel styling
  - `.settings-content` - Right content area
  - `.settings-header` - Category title header with border
  - `.settings-body` - Scrollable content area
  - `.settings-section` - Individual setting group container
  - `.language-option` - Language selection layout
- Any new styling should be added to `app.css` to maintain separation of concerns

### Culture Switching
- Culture is now managed through the Application Settings dialog (Language category)
- Switching culture updates both `DefaultThreadCurrentCulture` and `DefaultThreadCurrentUICulture`
- Changes are persisted to the user's local JSON settings file and take effect after saving
- Components inheriting from `ExtendedComponentBase` automatically re-render when culture changes

### Page Titles
Page components should inherit from `ExtendedComponentBase` and use the `SetTitle()` helper method:

**Recommended approach (using ExtendedComponentBase):**
```razor
@page "/appointments"
@inherits ExtendedComponentBase

@code {
    protected override void OnInitialized()
    {
        base.OnInitialized();  // IMPORTANT: Always call base first!
        SetTitle("AppointmentsPageTitle");
    }
}
```

Then add the corresponding keys to your resource files:
- `SharedResource.resx`: `AppointmentsPageTitle = "Appointments"`
- `SharedResource.bg-BG.resx`: `AppointmentsPageTitle = "Назначения"`

The title will automatically be displayed in the centered page title section and will update when culture changes.

## Development Notes
- Focus on CRUD operations for appointment and dismissal entries
- Windows-only deployment target
- Labour code articles are tracked as strings for flexibility
- All UI styling should go into `wwwroot/css/app.css` file

### Application Settings
The application uses a per-user settings system that controls various behaviors:
- **Settings Storage**: JSON file in user's local AppData (`%LOCALAPPDATA%\StaffTracker\app_settings.json`)
- **Per-User**: Each network user has their own settings file on their local machine
- **Access Pattern**: Inject `AppSettingsService` singleton and use `GetAsync()` to retrieve settings
- **User Interface**: Settings dialog accessible via topbar settings icon (⚙️)

**Settings Impact:**
- **Grid State Settings**: Control which aspects of grid state are saved/restored
  - Only enabled grid state aspects are persisted by `EntryGridStateService`
  - Example: If `GridStateFiltersSaving` is false, filters won't be saved or restored
- **Form Settings**: Control form behavior in Create/Edit pages
  - `FormCreateNew`: Auto-creates new blank form after successful save
  - `FormField*` settings: Persist specific field values across form sessions
- **Culture Setting**: Controls UI language and automatically updates on save

### Grid State Persistence
The application saves and restores MudDataGrid state (sorting, filtering, pagination, hidden columns) for the All Entries page:
- State persists across navigation and app restarts
- **User Control**: Grid state persistence is controlled by settings in the AppSettings dialog
- **Per-User**: Each network user has their own grid state on their local machine
- Hybrid save approach: 2-second timer + navigation events + component disposal
- State stored in JSON file at `%LOCALAPPDATA%\StaffTracker\entry_grid_state.json` (per-user)
- Thread-safe with lazy initialization to prevent deadlocks
- **Important**: Avoid blocking async calls in event handlers and Dispose methods
  - Navigation events use `async void` pattern
  - Dispose uses `Task.Run().Wait()` to avoid UI thread deadlock

## Deployment and Publishing

### Publishing the Application

The application uses framework-dependent deployment, which requires .NET 9.0 Runtime to be installed on client machines but produces a smaller deployment package.

Navigate to the repository root directory and run the following PowerShell command:

```powershell
dotnet publish .\StaffTracker\StaffTracker.csproj -f net9.0-windows10.0.19041.0 -c Release -p:WindowsPackageType=None -p:Platform=x64 -o .\StaffTracker\bin\Release\net9.0-windows10.0.19041.0\win-x64\publish
```

### Command Parameters Explained
- `-f net9.0-windows10.0.19041.0` - Specifies the target framework for Windows
- `-c Release` - Builds in Release configuration (optimized)
- `-p:WindowsPackageType=None` - Creates an unpackaged Windows app
- `-p:Platform=x64` - Targets 64-bit Windows systems
- `-o` - Output directory for published files

### Published Output Location
The command will create the published application in:
```
.\StaffTracker\bin\Release\net9.0-windows10.0.19041.0\win-x64\publish\
```

### Deployment to Network Share
After publishing, copy the contents of the publish folder to your network share location (e.g., `\\server\apps\StaffTracker\`). Create shortcuts on client machines pointing to the `StaffTracker.exe` file in the network location.

**Prerequisites:**
- .NET 9.0 Runtime must be installed on all client machines that will run the application

**Important Notes:**
- The database will be created in the `Database` subfolder relative to the executable location (on the network share)
- User-specific settings and grid state are stored locally on each user's machine in `%LOCALAPPDATA%\StaffTracker\`
- Only one user should access the application at a time to avoid database conflicts
