using Newtonsoft.Json;

namespace CalendarificTestHarness.Entities
{
    public class Datetime
    {
        [JsonProperty("year")]
        public int Year;

        [JsonProperty("month")]
        public int Month;

        [JsonProperty("day")]
        public int Day;

        [JsonProperty("hour")]
        public int? Hour;

        [JsonProperty("minute")]
        public int? Minute;

        [JsonProperty("second")]
        public int? Second;
    }
}