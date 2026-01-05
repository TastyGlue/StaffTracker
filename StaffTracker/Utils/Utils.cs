using StaffTracker.Models;
using System.Linq.Expressions;
using System.Text;

namespace StaffTracker.Utils;

public static class Utils
{
    public static bool TryGetEnumValue<TEnum>(int value, out TEnum result)
    where TEnum : struct, Enum
    {
        if (Enum.IsDefined(typeof(TEnum), value))
        {
            result = (TEnum)Enum.ToObject(typeof(TEnum), value);
            return true;
        }

        result = default;
        return false;
    }

    public static Func<EntryBase, object?> CreatePropertySelector(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(EntryBase), "x");
        var property = Expression.Property(parameter, propertyName);
        var converted = Expression.Convert(property, typeof(object));
        var lambda = Expression.Lambda<Func<EntryBase, object?>>(converted, parameter);
        return lambda.Compile();
    }

    public static string GetFullExceptionMessage(Exception ex)
    {
        if (ex == null) return string.Empty;

        var errorMessage = new StringBuilder();
        errorMessage.AppendLine($"Exception: {ex.Message}");
        errorMessage.AppendLine($"Stack Trace: {ex.StackTrace}");

        var innerException = ex.InnerException;
        while (innerException != null)
        {
            errorMessage.AppendLine("---- Inner Exception ----");
            errorMessage.AppendLine($"Exception: {innerException.Message}");
            errorMessage.AppendLine($"Stack Trace: {innerException.StackTrace}");
            innerException = innerException.InnerException;
        }

        return errorMessage.ToString();
    }

    public static string GetDefaultDownloadFolder()
    {
        try
        {
            var downloadsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads"
            );
            if (Directory.Exists(downloadsPath))
            {
                return downloadsPath;
            }
            else
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }
        catch
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
    }
}
