using StaffTracker.Enums;
using StaffTracker.Models;

namespace StaffTracker.Data;

public class DatabaseInitializer
{
    private readonly AppDbContext _context;

    public DatabaseInitializer(AppDbContext context)
    {
        _context = context;
    }

    public async Task InitializeAsync()
    {
        // Ensure the database is created
        await _context.Database.EnsureCreatedAsync().ConfigureAwait(false);

        // Alternatively, use migrations:
        // await _context.Database.MigrateAsync();
    }

    public async Task SeedTestDataAsync()
    {
        // Check if data already exists
        if (await _context.Appointments.AnyAsync().ConfigureAwait(false) || await _context.Dismissals.AnyAsync().ConfigureAwait(false))
        {
            return; // Database already seeded
        }

        // Seed 5 Appointments
        var appointments = new List<Appointment>
        {
            new Appointment
            {
                EntryType = EntryType.Appointment,
                EntryDate = new DateTime(2025, 1, 15),
                ConsideredFromDate = new DateTime(2025, 1, 15),
                IsNRAConfirmed = true,
                CompanyName = "ТехноСофт ООД",
                Division = "ИТ Отдел",
                IDN = "9501015678",
                FirstName = "Иван",
                SecondName = "Петров",
                Surname = "Иванов",
                Salary = 2500.00m,
                Currency = Currency.BGN,
                Position = "Програмист",
                ContractDate = new DateTime(2025, 1, 15),
                WorkExperienceDays = 1825,
                WorkingHours = 8,
                IdCardNumber = "123456789",
                IdCardDate = new DateTime(2020, 5, 10),
                Address = "гр. София, ул. Витоша 15"
            },
            new Appointment
            {
                EntryType = EntryType.Appointment,
                EntryDate = new DateTime(2025, 2, 1),
                ConsideredFromDate = new DateTime(2025, 2, 1),
                IsNRAConfirmed = false,
                CompanyName = "БизнесКонсулт ЕООД",
                IDN = "8803124567",
                FirstName = "Мария",
                Surname = "Георгиева",
                Salary = 1800.50m,
                Currency = Currency.BGN,
                Position = "Счетоводител",
                ContractDate = new DateTime(2025, 2, 1),
                WorkingHours = 8
            },
            new Appointment
            {
                EntryType = EntryType.Appointment,
                EntryDate = new DateTime(2025, 3, 10),
                ConsideredFromDate = new DateTime(2025, 3, 10),
                IsNRAConfirmed = true,
                CompanyName = "Строй Инженеринг АД",
                Division = "Проектиране",
                IDN = "9205203456",
                FirstName = "Георги",
                SecondName = "Стоянов",
                Surname = "Димитров",
                Salary = 3200.00m,
                Currency = Currency.BGN,
                Position = "Главен инженер",
                ContractDate = new DateTime(2025, 3, 10),
                WorkExperienceDays = 3650,
                WorkExperienceInProfessionDays = 2920,
                WorkingHours = 8,
                Address = "гр. Пловдив, бул. Марица 45"
            },
            new Appointment
            {
                EntryType = EntryType.Appointment,
                EntryDate = new DateTime(2025, 4, 5),
                ConsideredFromDate = new DateTime(2025, 4, 5),
                IsNRAConfirmed = true,
                CompanyName = "МедиЦентър ЕООД",
                IDN = "9709155234",
                FirstName = "Елена",
                Surname = "Петкова",
                Salary = 2200.00m,
                Currency = Currency.BGN,
                Position = "Медицинска сестра",
                ContractDate = new DateTime(2025, 4, 5),
                WorkingHours = 12,
                IdCardNumber = "987654321",
                IdCardDate = new DateTime(2022, 3, 15)
            },
            new Appointment
            {
                EntryType = EntryType.Appointment,
                EntryDate = new DateTime(2025, 5, 20),
                ConsideredFromDate = new DateTime(2025, 5, 20),
                IsNRAConfirmed = false,
                CompanyName = "Образование Плюс ООД",
                Division = "Начално образование",
                IDN = "8512254321",
                FirstName = "Стоян",
                SecondName = "Николов",
                Surname = "Христов",
                Salary = 1950.00m,
                Currency = Currency.BGN,
                Position = "Учител",
                ContractDate = new DateTime(2025, 5, 20),
                WorkExperienceDays = 2555,
                WorkingHours = 8
            }
        };

        // Seed 5 Dismissals
        var dismissals = new List<Dismissal>
        {
            new Dismissal
            {
                EntryType = EntryType.Dismissal,
                EntryDate = new DateTime(2025, 1, 31),
                ConsideredFromDate = new DateTime(2025, 1, 31),
                IsNRAConfirmed = true,
                CompanyName = "Старт АД",
                IDN = "7801105432",
                FirstName = "Петър",
                Surname = "Тодоров",
                LabourCodeArticle = 328,
                LabourCodeParagraph = 1,
                LabourCodeItem = 2,
                CompensationDays = 30
            },
            new Dismissal
            {
                EntryType = EntryType.Dismissal,
                EntryDate = new DateTime(2025, 2, 15),
                ConsideredFromDate = new DateTime(2025, 2, 15),
                IsNRAConfirmed = true,
                CompanyName = "Търговия ЕООД",
                Division = "Склад",
                IDN = "9203254567",
                FirstName = "Анна",
                SecondName = "Василева",
                Surname = "Стоянова",
                LabourCodeArticle = 325,
                LabourCodeParagraph = 1,
                LabourCodeItem = 1,
                Garnishment = false,
                LeaveLastMonthDays = 5
            },
            new Dismissal
            {
                EntryType = EntryType.Dismissal,
                EntryDate = new DateTime(2025, 3, 20),
                ConsideredFromDate = new DateTime(2025, 3, 20),
                IsNRAConfirmed = false,
                CompanyName = "ПродуктиКо ООД",
                IDN = "8608123456",
                FirstName = "Николай",
                Surname = "Александров",
                LabourCodeArticle = 330,
                LabourCodeParagraph = 2,
                LabourCodeItem = 6,
                CompensationDays = 0,
                Garnishment = true
            },
            new Dismissal
            {
                EntryType = EntryType.Dismissal,
                EntryDate = new DateTime(2025, 4, 10),
                ConsideredFromDate = new DateTime(2025, 4, 10),
                IsNRAConfirmed = true,
                CompanyName = "Логистик Експрес АД",
                Division = "Транспорт",
                IDN = "9012305678",
                FirstName = "Димитър",
                SecondName = "Христов",
                Surname = "Йорданов",
                LabourCodeArticle = 327,
                LabourCodeParagraph = 1,
                CompensationDays = 15,
                LeaveLastMonthDays = 2
            },
            new Dismissal
            {
                EntryType = EntryType.Dismissal,
                EntryDate = new DateTime(2025, 5, 1),
                ConsideredFromDate = new DateTime(2025, 5, 1),
                IsNRAConfirmed = false,
                CompanyName = "Финанси Консулт ООД",
                IDN = "8907156789",
                FirstName = "Светлана",
                Surname = "Иванова",
                LabourCodeArticle = 328,
                LabourCodeParagraph = 1,
                LabourCodeItem = 11,
                CompensationDays = 60,
                Garnishment = false,
                LeaveLastMonthDays = 10
            }
        };

        await _context.Appointments.AddRangeAsync(appointments).ConfigureAwait(false);
        await _context.Dismissals.AddRangeAsync(dismissals).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}
