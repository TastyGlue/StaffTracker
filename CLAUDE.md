# Dismissal_Appointment - Employee Accounting Application

## Project Overview
A simple CRUD accounting application built with .NET MAUI Blazor Hybrid, running exclusively on Windows. The application tracks appointments (hiring) and dismissals of employees for accounting purposes.

## Technology Stack
- **Framework**: .NET MAUI Blazor Hybrid
- **Platform**: Windows only
- **Language**: C#
- **Database**: SQLite with Entity Framework Core 9.0
- **Data Storage**: Local file in user's AppData\Local folder

## Project Structure

### Models (`Dismissal_Appointment/Models/`)

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
Model for application-wide settings stored in the database:
- `Id` (int) - Primary key (always 1, ensuring single row constraint)
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

### Base Components (`Dismissal_Appointment/Components/Pages/Abstract/`)

#### ExtendedComponentBase.cs
Abstract base class for all page components that provides common functionality:
- Inherits from `ComponentBase` and implements `IDisposable`
- Automatically injects `ILocalizationService` (as `Localizer` property)
- Automatically injects `IPageTitleService` (as `PageTitleService` property)
- Automatically subscribes to culture change and page title change events in `OnInitialized()`
- Provides `SetTitle(string title)` helper method for setting page titles
- Automatically handles cleanup and unsubscribes from events in `Dispose()`
- **IMPORTANT**: When overriding `OnInitialized()` or other lifecycle methods, always call `base.OnInitialized()` first to ensure event subscriptions work correctly

### Enums (`Dismissal_Appointment/Enums/`)

#### EntryType.cs
Defines entry types:
- `Dismissal = 1`
- `Appointment = 2`

### Data (`Dismissal_Appointment/Data/`)

#### AppDbContext.cs
Entity Framework Core DbContext for the application:
- `DbSet<EntryBase> Entries` - Collection of all entry records (base type)
- `DbSet<Appointment> Appointments` - Collection of appointment records
- `DbSet<Dismissal> Dismissals` - Collection of dismissal records
- `DbSet<AppSettings> AppSettings` - Application settings (single row with Id=1)
- Uses Table-Per-Type (TPT) mapping strategy for inheritance hierarchy
- Configures decimal precision for salary fields
- Enforces required properties and constraints

#### DatabaseConfig.cs
Configuration helper for database connection:
- `DatabasePath` - Returns the full path to the SQLite database file
- `ConnectionString` - Provides the SQLite connection string
- Database location: `%LOCALAPPDATA%\dismissal_appointment.db`

#### DatabaseInitializer.cs
Service for database initialization:
- Ensures database is created on application startup
- Called synchronously in `App.xaml.cs` constructor before any pages are created
- Uses `EnsureCreatedAsync()` to create database schema
- Seeds default `AppSettings` (culture, grid state flags, form flags)
- Seeds test data (5 appointments and 5 dismissals) on first run
- Registered as transient service in dependency injection

### Services (`Dismissal_Appointment/Services/`)

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
- JSON persistence to `{AppContext.BaseDirectory}\entry_grid_state.json`
- Camel case JSON naming policy for readability
- Automatic state saving and loading

**Storage:**
- Persists grid state to JSON file in the application directory
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
Service for managing application-wide settings:
- Provides business logic for retrieving and updating application settings
- Works with `AppSettingsStateContainer` for caching
- Registered as **scoped** service in dependency injection container

**Public API:**
```csharp
Task<AppSettings?> GetAsync()  // Retrieves current settings (cached or from database)
Task UpdateAsync(AppSettings settings)  // Updates and persists settings to database
```

**Usage:**
Settings control various application behaviors:
- Language/culture selection
- Grid state persistence options (sorting, filtering, paging, hidden columns)
- Form behavior (auto-create new, remember field values)

#### AppSettingsStateContainer.cs
In-memory cache for application settings:
- Prevents unnecessary database queries by caching settings in memory
- Thread-safe operations using `SemaphoreSlim`
- Registered as **singleton** service (application-wide)

**Features:**
- Caches settings after first database load
- Automatically cleared and reloaded when settings are updated
- Thread-safe get, set, and clear operations

### Resources (`Dismissal_Appointment/Resources/Translations/`)

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
The SQLite database is stored locally on the user's machine at:
```
C:\Users\[Username]\AppData\Local\dismissal_appointment.db
```

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
- **AppSettings** - Application settings (single row with Id=1)

### Default Settings
The `AppSettings` table is seeded with default values on first run:
```csharp
{
    Id = 1,
    Culture = "bg-BG",
    GridStateSortsSaving = true,
    GridStateFiltersSaving = true,
    GridStatePageSizeSaving = true,
    GridStatePageIndexSaving = true,
    GridStateHiddenColumnsSaving = true,
    FormCreateNew = false,
    FormFieldEntryDate = false,
    FormFieldCompany = false,
    FormFieldDivision = false
}
```

### NuGet Packages
- `Microsoft.EntityFrameworkCore.Sqlite` (v9.0.11)
- `Microsoft.EntityFrameworkCore.Tools` (v9.0.11)

## Application Startup

### Initialization Sequence
The application follows a specific initialization order to ensure the database is ready before any Blazor pages load:

1. **`App.xaml.cs` Constructor** (`Dismissal_Appointment/App.xaml.cs`)
   - Called when the MAUI application starts
   - Initializes database **synchronously** using `InitializeDatabaseAsync().GetAwaiter().GetResult()`
   - Ensures database creation and seeding complete before proceeding
   - Initializes localization from saved `AppSettings` culture preference
   - Uses scoped service provider to avoid singleton/scoped service conflicts

2. **Database Initialization** (via `DatabaseInitializer`)
   - Creates database schema if it doesn't exist
   - Seeds default `AppSettings` (Id=1) with Bulgarian culture and default flags
   - Seeds test data (5 appointments, 5 dismissals) on first run

3. **Localization Initialization**
   - Loads saved culture from `AppSettings` table
   - Sets `DefaultThreadCurrentCulture` and `DefaultThreadCurrentUICulture`
   - Falls back to Bulgarian (bg-BG) if no settings exist

4. **MainPage Creation** (`Dismissal_Appointment/MainPage.xaml.cs`)
   - Simple ContentPage with BlazorWebView
   - No initialization logic (handled by App.xaml.cs)
   - Database is guaranteed to exist at this point

5. **Blazor UI Loads**
   - Home page (All.razor.cs) can safely query database
   - Grid state is restored from JSON file
   - Application is ready for user interaction

### Critical Design Notes
- **Synchronous initialization**: Using `.GetAwaiter().GetResult()` in App constructor ensures database exists before any UI loads
- **Race condition prevention**: Previous async `Task.Run()` approach in MainPage caused "no such table" errors
- **Service scoping**: Database initialization uses `CreateScope()` to properly resolve scoped services (`AppDbContext`) from the singleton App instance

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
- Changes are persisted to the database and take effect after saving the settings
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
The application uses a centralized settings system that controls various behaviors:
- **Settings Storage**: Single-row table in SQLite database (Id=1)
- **Caching**: Settings are cached in memory via `AppSettingsStateContainer` singleton
- **Access Pattern**: Inject `AppSettingsService` and use `GetAsync()` to retrieve settings
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
- Hybrid save approach: 2-second timer + navigation events + component disposal
- State stored in JSON file at `{AppContext.BaseDirectory}\entry_grid_state.json`
- Thread-safe with lazy initialization to prevent deadlocks
- **Important**: Avoid blocking async calls in event handlers and Dispose methods
  - Navigation events use `async void` pattern
  - Dispose uses `Task.Run().Wait()` to avoid UI thread deadlock
