using Newtonsoft.Json;

namespace CalendarificTestHarness.Entities
{
    public class Country
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("name")]
        public string Name;
    }
}