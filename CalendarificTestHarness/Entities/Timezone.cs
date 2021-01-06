using Newtonsoft.Json;

namespace CalendarificTestHarness.Entities
{
    public class Timezone
    {
        [JsonProperty("offset")]
        public string Offset;

        [JsonProperty("zoneabb")]
        public string ZoneAbb;

        [JsonProperty("zoneoffset")]
        public int ZoneOffset;

        [JsonProperty("zonedst")]
        public int ZoneDst;

        [JsonProperty("zonetotaloffset")]
        public int ZoneTotalOffset;
    }
}