using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Circle.Game.Converting.Json
{
    public class FloatToIntConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) => isFloatingPoint(typeToConvert);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (!isFloatingPoint(typeToConvert))
                throw new InvalidOperationException($"Cannot convert {typeToConvert} to int");

            var converter = (JsonConverter)Activator.CreateInstance(typeof(FloatingPointToIntConverter<>).MakeGenericType(typeToConvert))!;

            return converter;
        }

        private bool isFloatingPoint(Type typeToConvert)
        {
            return typeToConvert == typeof(float) ||
                   typeToConvert == typeof(double) ||
                   typeToConvert == typeof(decimal);
        }

        private class FloatingPointToIntConverter<T> : JsonConverter<T>
            where T : struct
        {
            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return (T)Convert.ChangeType(reader.GetDouble(), typeof(T));
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                int intValue = Convert.ToInt32(value);
                double doubleValue = Convert.ToDouble(value);

                writer.WriteNumberValue(doubleValue - intValue == 0 ? intValue : doubleValue);
            }
        }
    }
}
