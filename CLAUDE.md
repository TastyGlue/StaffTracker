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

### Enums (`Dismissal_Appointment/Enums/`)

#### EntryType.cs
Defines entry types:
- `Dismissal = 1`
- `Appointment = 2`

### Data (`Dismissal_Appointment/Data/`)

#### AppDbContext.cs
Entity Framework Core DbContext for the application:
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

## Development Notes
- Focus on CRUD operations for appointment and dismissal entries
- Windows-only deployment target
- Labour code articles are tracked as strings for flexibility
