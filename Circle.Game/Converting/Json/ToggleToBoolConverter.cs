using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Circle.Game.Converting.Json
{
    public class ToggleToBoolConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.True) return true;
            if (reader.TokenType == JsonTokenType.False) return false;

            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException();

            string stringValue = reader.GetString() ?? string.Empty;

            switch (stringValue.ToLower(CultureInfo.CurrentCulture))
            {
                case "disabled":
                case "off":
                case "0":
                case "false":
                    return false;

                case "enabled":
                case "on":
                case "1":
                case "true":
                    return true;
            }

            throw new JsonException($"Cannot convert value '{stringValue}' to boolean");
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }
}
