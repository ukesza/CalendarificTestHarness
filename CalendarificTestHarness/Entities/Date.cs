using System;
using Newtonsoft.Json;

namespace CalendarificTestHarness.Entities
{
    public class Date
    {
        [JsonProperty("iso")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime DateTime;

        [JsonProperty("datetime")]
        public Datetime Datetime;

        [JsonProperty("timezone")]
        public Timezone Timezone;
    }
}