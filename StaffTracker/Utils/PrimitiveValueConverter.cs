using StaffTracker.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StaffTracker.Utils;

public sealed class PrimitiveValueConverter : JsonConverter<object>
{
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => ReadString(reader),
            JsonTokenType.Number => reader.TryGetInt32(out var l) ? l : reader.GetDecimal(),
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.Null => null,
            _ => throw new JsonException($"Unsupported token: {reader.TokenType}")
        };
    }

    private static object ReadString(Utf8JsonReader reader)
    {
        // Prefer DateTime if it parses cleanly
        if (reader.TryGetDateTime(out DateTime dateTime))
            return dateTime;

        return reader.GetString()!;
    }

    public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        switch (value)
        {
            case DateTime dt:
                writer.WriteStringValue(dt);
                break;

            case DateTimeOffset dto:
                writer.WriteStringValue(dto);
                break;

            case bool b:
                writer.WriteBooleanValue(b);
                break;

            case int i:
                writer.WriteNumberValue(i);
                break;

            case long l:
                writer.WriteNumberValue(l);
                break;

            case double d:
                writer.WriteNumberValue(d);
                break;

            case decimal dec:
                writer.WriteNumberValue(dec);
                break;

            case string s:
                writer.WriteStringValue(s);
                break;

            case EntryType et:
                writer.WriteNumberValue((int)et);
                break;

            default:
                throw new JsonException($"Unsupported type: {value.GetType()}");
        }
    }
}