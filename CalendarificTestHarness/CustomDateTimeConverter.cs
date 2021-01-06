using System;
using Newtonsoft.Json;

namespace CalendarificTestHarness
{
    public class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            return DateTime.Parse(reader.Value.ToString());
        }
    }
}