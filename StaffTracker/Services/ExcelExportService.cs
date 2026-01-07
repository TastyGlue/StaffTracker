using OfficeOpenXml;
using OfficeOpenXml.Style;
using Color = System.Drawing.Color;

namespace StaffTracker.Services;

/// <summary>
/// Service for exporting employee entries to Excel files with Bulgarian formatting
/// </summary>
public class ExcelExportService : IExportService
{
    private static readonly Color YellowHighlight = Color.Yellow;

    public async Task ExportToExcelAsync(IEnumerable<EntryBase> entries, string filePath)
    {
        // Set EPPlus license context
        ExcelPackage.License.SetNonCommercialOrganization("Sitikom 2007");

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Записи");

        int currentRow = 1;

        foreach (var entry in entries)
        {
            if (entry is Appointment appointment)
            {
                currentRow = WriteAppointmentEntry(worksheet, appointment, currentRow);
            }
            else if (entry is Dismissal dismissal)
            {
                currentRow = WriteDismissalEntry(worksheet, dismissal, currentRow);
            }

            // Add spacing between entries
            currentRow += 2;
        }

        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();

        // Set minimum column widths to ensure merged cells display properly
        // AutoFit doesn't handle merged cells well, so we ensure a minimum width
        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
        {
            double currentWidth = worksheet.Column(col).Width;
            // Set minimum width of 20 for better display of merged cells
            if (currentWidth < 20)
            {
                worksheet.Column(col).Width = 20;
            }
        }

        // Save the file
        var fileInfo = new FileInfo(filePath);
        await package.SaveAsAsync(fileInfo);
    }

    private static int WriteAppointmentEntry(ExcelWorksheet worksheet, Appointment appointment, int startRow)
    {
        int row = startRow;

        // Row 1: Header "За назначаване"
        var headerCell = worksheet.Cells[row, 1];
        headerCell.Value = "За назначаване";
        headerCell.Style.Font.Bold = true;
        headerCell.Style.Font.Color.SetColor(Color.Red);
        row++;

        // Row 2: ФИРМА ЗА НАЗНАЧАВАНЕ | company value | ПОДЕЛЕНИЕ | division value
        SetBoldCell(worksheet.Cells[row, 1], appointment.CompanyName);

        worksheet.Cells[row, 2].Value = appointment.Division ?? "";
        SetBorderedCell(worksheet.Cells[row, 2]);
        row++;

        // Row 3: ЕГН | IDN | ИМЕ | first name | ПРЕЗИМЕ | second name | ФАМИЛИЯ | surname
        SetBoldCell(worksheet.Cells[row, 1], appointment.IDN);

        SetBoldCell(worksheet.Cells[row, 2], appointment.FullName);
        row++;

        // Row 4: ЗАПЛАТА | ДЛЪЖНОСТ | ТРУДОВ СТАЖ
        SetBoldCell(worksheet.Cells[row, 1], FormatSalary(appointment.Salary, appointment.Currency));

        SetBoldCell(worksheet.Cells[row, 2], $"ДЛЪЖНОСТ - {appointment.Position}");

        var serviceCell = worksheet.Cells[row, 3, row, 5];
        serviceCell.Merge = true;
        serviceCell.Value = FormatWorkExperienceDays(appointment.WorkExperienceDays);
        SetBorderedCell(serviceCell);
        row++;

        // Row 5: ТРУДОВ СТАК ПРОФЕСИЯ (empty for now)
        var professionCell = worksheet.Cells[row, 3, row, 5];
        professionCell.Merge = true;
        professionCell.Value = FormatWorkExperienceJobDays(appointment.WorkExperienceInProfessionDays);
        SetBorderedCell(professionCell);
        row++;

        // Row 6: СЧИТАНО ОТ ДАТА | ДАТА НА ДОГОВОР | часове работен ден
        SetBoldCell(worksheet.Cells[row, 1], $"СЧИТАНО ОТ ДАТА - {FormatDate(appointment.EntryDate)}");

        worksheet.Cells[row, 2].Value = $"ДАТА НА ДОГОВОР - {FormatDate(appointment.ConsideredFromDate)}";
        SetBorderedCell(worksheet.Cells[row, 2]);

        var workHoursCell = worksheet.Cells[row, 3, row, 5];
        workHoursCell.Merge = true;
        workHoursCell.Value = $"часове работен ден - {FormatNullableInt(appointment.WorkingHours)}";
        SetBorderedCell(workHoursCell);
        row++;

        // Row 7: ЛИЧНА КАРТА № | ДАТА НА ИЗДАВАНЕ
        worksheet.Cells[row, 1].Value = $"ЛК № - {appointment.IdCardNumber ?? "NULL"}";
        SetBorderedCell(worksheet.Cells[row, 1]);

        worksheet.Cells[row, 2].Value = $"ДАТА НА ИЗДАВАНЕ - {FormatDate(appointment.IdCardDate)}";
        SetBorderedCell(worksheet.Cells[row, 3]);

        var authorityCell = worksheet.Cells[row, 3, row, 5]; 
        authorityCell.Merge = true;
        authorityCell.Value = $"ИЗДАДЕН ОТ - {appointment.IdCardAuthority ?? "NULL"}";
        SetBorderedCell(authorityCell);
        row++;

        // Row 8: АДРЕС
        var addressCell = worksheet.Cells[row, 1, row, 5];
        addressCell.Merge = true;
        addressCell.Value = $"АДРЕС - {appointment.Address ?? "NULL"}";
        SetBorderedCell(addressCell);
        row++;

        return row;
    }

    private static int WriteDismissalEntry(ExcelWorksheet worksheet, Dismissal dismissal, int startRow)
    {
        int row = startRow;

        // Row 1: Header "За уволнение"
        var headerCell = worksheet.Cells[row, 1];
        headerCell.Value = "За уволнение";
        headerCell.Style.Font.Bold = true;
        headerCell.Style.Font.Color.SetColor(Color.Red);
        row++;

        // Row 2: ФИРМА ЗА УВОЛНЕНИЕ | company value | ПОДЕЛЕНИЕ | division value
        SetBoldCell(worksheet.Cells[row, 1], dismissal.CompanyName);

        worksheet.Cells[row, 2].Value = dismissal.Division ?? "";
        SetBorderedCell(worksheet.Cells[row, 2]);
        row++;

        // Row 3: ЕГН | IDN | ИМЕ | first name | ПРЕЗИМЕ | second name | ФАМИЛИЯ | surname
        SetBoldCell(worksheet.Cells[row, 1], dismissal.IDN);

        SetBoldCell(worksheet.Cells[row, 2], dismissal.FullName);
        row++;

        // Row 4: СЧИТАНО ОТ ДАТА
        SetBoldCell(worksheet.Cells[row, 1], $"СЧИТАНО ОТ ДАТА - {FormatDate(dismissal.EntryDate)}");
        row++;

        // Row 5: ЧЛЕН ОТ КТ | ОБЕЗЩЕТЕНИЕ ПО ЧЛ.224-ДНИ
        string labourCodeArticle = FormatLabourCodeArticle(dismissal.LabourCodeArticle, dismissal.LabourCodeParagraph, dismissal.LabourCodeItem);
        worksheet.Cells[row, 1].Value = $"ЧЛЕН ОТ КТ - {labourCodeArticle}";
        SetBorderedCell(worksheet.Cells[row, 1]);

        var compensationCell = worksheet.Cells[row, 2, row, 3];
        compensationCell.Merge = true;
        compensationCell.Value = $"ОБЕЗЩЕТЕНИЕ ПО ЧЛ.224 - {dismissal.CompensationDays.ToString() ?? "NULL"} ДНИ";
        SetBorderedCell(compensationCell);
        row++;

        // Row 6: ЗАПОР - ДА, НЕ | ОТПУСК ПОСЛЕДЕН МЕСЕЦ
        string garnishment = $"ЗАПОР - ";
        worksheet.Cells[row, 1].Value = FormatGarnishment(dismissal.Garnishment);
        SetBorderedCell(worksheet.Cells[row, 1]);

        var leaveCell = worksheet.Cells[row, 2, row, 3];
        leaveCell.Merge = true;
        leaveCell.Value = $"ОТПУСК ПОСЛЕДЕН МЕСЕЦ - {FormatNullableInt(dismissal.LeaveLastMonthDays)}";
        SetBorderedCell(leaveCell);
        row++;

        return row;
    }

    private static void SetBoldCell(ExcelRange cell, string value)
    {
        cell.Value = value;
        cell.Style.Font.Bold = true;
        SetBorderedCell(cell);
    }

    private static void SetBorderedCell(ExcelRange cell)
    {
        cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
    }

    private static string FormatDate(DateTime? date)
    {
        return date?.ToString("dd.MM.yyyy") ?? "NULL";
    }

    private static string FormatNullableInt(int? value)
    {
        return (value is not null) ? value.Value.ToString() : "NULL";
    }

    private static string FormatSalary(decimal salary, Currency currency)
    {
        return $"ЗАПЛАТА - {salary:F2} {currency}";
    }

    private static string FormatWorkExperienceDays(int? days)
    {
        if (!days.HasValue || days.Value == 0)
            return "ТРУДОВ СТАЖ ОБЩ години / месеци / дни";

        int years = days.Value / 365;
        int remainingDays = days.Value % 365;
        int months = remainingDays / 30;
        int finalDays = remainingDays % 30;

        return $"ТРУДОВ СТАЖ ОБЩ - {years} години / {months} месеци / {finalDays} дни";
    }

    private static string FormatWorkExperienceJobDays(int? days)
    {
        if (!days.HasValue || days.Value == 0)
            return "ТРУДОВ СТАЖ ПРОФЕСИЯ години / месеци / дни";

        int years = days.Value / 365;
        int remainingDays = days.Value % 365;
        int months = remainingDays / 30;
        int finalDays = remainingDays % 30;

        return $"ТРУДОВ СТАЖ ПРОФЕСИЯ - {years} години / {months} месеци / {finalDays} дни";
    }

    private static string FormatLabourCodeArticle(int labourCodeArticle, int? labourCodeParagraph, int? labourCodeItem)
    {
        string result = $"Чл.{labourCodeArticle}";
        if (labourCodeParagraph is not null)
            result += $", п.{labourCodeParagraph}";
        if (labourCodeItem is not null)
            result += $", т.{labourCodeItem}";

        return result;
    }

    private static string FormatGarnishment(bool? garnishment)
    {
        if (garnishment is not null)
            return (garnishment.Value) ? "ДА" : "НЕ";
        else
            return "NULL";
    }
}
