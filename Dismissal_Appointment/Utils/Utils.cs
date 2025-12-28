using System.Linq.Expressions;

namespace Dismissal_Appointment.Utils;

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
}
