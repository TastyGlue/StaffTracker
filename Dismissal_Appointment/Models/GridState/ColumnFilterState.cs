using System.Text.Json.Serialization;

namespace Dismissal_Appointment.Models.GridState;

public class ColumnFilterState
{
    public string PropertyName { get; set; } = default!;
    public string Operator { get; set; } = default!;

    [JsonConverter(typeof(PrimitiveValueConverter))]
    public object? Value { get; set; }
}
