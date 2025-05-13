using System;
using Newtonsoft.Json;

namespace Circle.Game.Converting.Json
{
    public class FloatToIntConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            double floatingPointValue = Convert.ToDouble(value);
            int integerValue = Convert.ToInt32(value);

            writer.WriteValue(floatingPointValue - integerValue == 0 ? integerValue : value);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, objectType);
        }

        public override bool CanConvert(Type objectType)
        {
            return isNumericType(objectType);
        }

        private bool isNumericType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
            }

            return false;
        }
    }
}
