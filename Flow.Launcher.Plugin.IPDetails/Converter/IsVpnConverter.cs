using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class IsVpnConverter : JsonConverter<object>
{
    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
        {
            return reader.GetBoolean();
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString();
        }

        throw new JsonException("Invalid type for 'is_vpn' field.");
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        if (value is bool boolValue)
        {
            writer.WriteBooleanValue(boolValue);
        }
        else if (value is string stringValue)
        {
            writer.WriteStringValue(stringValue);
        }
        else
        {
            throw new JsonException("Invalid type for 'is_vpn' field.");
        }
    }
}