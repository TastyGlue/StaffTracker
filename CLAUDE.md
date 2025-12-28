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
- Called automatically when MainPage is constructed
- Uses `EnsureCreatedAsync()` to create database schema

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

### NuGet Packages
- `Microsoft.EntityFrameworkCore.Sqlite` (v9.0.11)
- `Microsoft.EntityFrameworkCore.Tools` (v9.0.11)

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
  - Culture/language switcher located in the right side (Bulgarian/English)
- **Page Title Section**: Centered horizontally below the topbar
  - Displays the current page title with clear separation from content
- **Page Content**: Main content area below the title section
  - Max-width of 1400px, centered horizontally
  - Light background color (`#fafafa`)

### Styling Guidelines
- All custom styling for the layout is defined in `wwwroot/css/app.css`
- Layout uses the following CSS classes:
  - `.app-topbar` - Styling for the top application bar
  - `.main-content` - Main content area background
  - `.page-title-container` - Container for centered page title
  - `.page-title` - Page title text styling
  - `.page-content` - Content area with padding and max-width
- Any new styling should be added to `app.css` to maintain separation of concerns

### Culture Switching
- Culture can be switched via the language menu in the topbar
- Switching culture updates both `DefaultThreadCurrentCulture` and `DefaultThreadCurrentUICulture`
- The active culture is highlighted with a checkmark icon and blue background
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

### Grid State Persistence
The application automatically saves and restores MudDataGrid state (sorting, filtering, pagination, hidden columns) for the All Entries page:
- State persists across navigation and app restarts
- Hybrid save approach: 2-second timer + navigation events + component disposal
- State stored in JSON file at `{AppContext.BaseDirectory}\entry_grid_state.json`
- Thread-safe with lazy initialization to prevent deadlocks
- **Important**: Avoid blocking async calls in event handlers and Dispose methods
  - Navigation events use `async void` pattern
  - Dispose uses `Task.Run().Wait()` to avoid UI thread deadlock
