using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Circle.Game.Converting.Json
{
    public class ExcludeIndentConverter<T> : JsonConverter<T>
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            string json = document.RootElement.GetRawText();

            if (document.RootElement.ValueKind != JsonValueKind.Object)
                return JsonSerializer.Deserialize<T>(json, options);

            // 범용적으로 만드려고 노력했지만 모든 케이스에서 잘 작동할지 모르겠음.
            object? result = Activator.CreateInstance(typeof(T));
            var props = typeof(T)
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.CanWrite);

            foreach (var prop in props)
            {
                if (document.RootElement.TryGetProperty(prop.Name, out var jsonProp))
                {
                    object? value = jsonProp.Deserialize(prop.PropertyType, options);
                    prop.SetValue(result, value);
                }
            }

            return (T)result!;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                if (options.DefaultIgnoreCondition != JsonIgnoreCondition.WhenWritingNull)
                    writer.WriteNullValue();

                return;
            }

            if (value is not string && isCollection(value.GetType()))
            {
                writer.WriteRawValue(writeArray((IEnumerable)value, options));
                return;
            }

            if (isPrimitive(value.GetType()))
            {
                writer.WriteRawValue($"{value}");
                return;
            }

            string indent = options.WriteIndented ? '\n' + string.Concat(Enumerable.Repeat(options.IndentCharacter, options.IndentSize * writer.CurrentDepth)) : string.Empty;
            writer.WriteRawValue($"{indent}{writeObject(value, options)}");
        }

        private string writeArray(IEnumerable array, JsonSerializerOptions options)
        {
            var arrayBuilder = new StringBuilder("[");
            bool first = true;

            foreach (object? element in array)
            {
                string rawValue = $"{element}";

                if (element == null && options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingNull)
                    continue;

                var type = element == null ? null : Nullable.GetUnderlyingType(element.GetType()) ?? element.GetType();

                switch (type)
                {
                    case null:
                    case var t when t.IsPrimitive:
                        if (type == typeof(bool))
                            rawValue = element?.ToString()?.ToLowerInvariant() ?? rawValue;

                        break;

                    case var t when t == typeof(string):
                        rawValue = $"\"{element}\"";
                        break;

                    case var t when t.IsEnum:
                        if (options.Converters.Any(c => c is JsonStringEnumConverter))
                            rawValue = $"\"{element}\"";
                        else
                            rawValue = $"{(int)element!}";

                        break;

                    case var t when t.IsArray:
                        arrayBuilder.Append(writeArray((IEnumerable)element!, options));
                        continue;

                    // Class or Struct
                    case var c when c.IsClass:
                    case var s when s.IsValueType && !s.IsPrimitive && !s.IsEnum:
                        arrayBuilder.Append(writeObject(element!, options));
                        continue;
                }

                if (!first)
                    arrayBuilder.Append(", ");

                arrayBuilder.Append(rawValue);

                first = false;
            }

            arrayBuilder.Append(']');

            return arrayBuilder.ToString();
        }

        private string writeObject(object obj, JsonSerializerOptions options)
        {
            var objectBuilder = new StringBuilder("{ ");
            bool first = true;

            foreach (var property in obj.GetType().GetProperties())
            {
                object? value = property.GetValue(obj);
                string rawValue = $"{value}";

                if (value == null && options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingNull)
                    continue;

                var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                if (value != null)
                {
                    switch (propertyType)
                    {
                        case Type t when t.IsPrimitive:
                            if (t == typeof(bool))
                                rawValue = value.ToString()!.ToLowerInvariant();

                            break;

                        case Type t when t == typeof(string):
                            rawValue = $"\"{value}\"";
                            break;

                        case Type t when t.IsEnum:
                            if (options.Converters.Any(c => c is JsonStringEnumConverter))
                                rawValue = $"\"{value}\"";
                            else
                                rawValue = $"{(int)value}";

                            break;

                        case Type t when t.IsArray:
                            rawValue = writeArray((IEnumerable)value, options);
                            break;

                        // Class or Struct
                        case Type c when c.IsClass:
                        case Type s when s.IsValueType && !s.IsPrimitive && !s.IsEnum:
                            rawValue = writeObject(value, options);
                            break;
                    }
                }

                if (!first)
                    objectBuilder.Append(", ");

                objectBuilder.Append($"\"{property.Name}\": {rawValue}");

                first = false;
            }

            objectBuilder.Append(" }");

            return objectBuilder.ToString();
        }

        private bool isCollection(Type type) => type.IsArray || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>));

        private bool isPrimitive(Type type) => type.IsPrimitive || type == typeof(string);
    }
}
