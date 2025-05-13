using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Circle.Game.Converting.Json
{
    public class ExcludeIndentConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var savedFormatting = writer.Formatting;
            writer.Formatting = Formatting.None;

            if (value.GetType().IsArray || value is IEnumerable)
            {
                writer.WriteStartArray();

                foreach (object item in (IEnumerable)value)
                    serializer.Serialize(writer, item);

                writer.WriteEndArray();
            }
            else
            {
                writer.Formatting = Formatting.Indented;

                writer.WriteStartObject();
                writer.Formatting = Formatting.None;

                foreach (var property in value.GetType().GetProperties())
                {
                    if (property.GetValue(value) == null && serializer.NullValueHandling == NullValueHandling.Ignore)
                        continue;

                    writer.WritePropertyName(property.Name, true);
                    serializer.Serialize(writer, property.GetValue(value));
                }

                writer.WriteEndObject();
            }

            writer.Formatting = savedFormatting;
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, objectType);
        }

        public override bool CanConvert(Type objectType) => isCollection(objectType) || objectType.IsClass;

        public override bool CanRead => false;

        private bool isCollection(Type type) => type.IsArray || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }
}
