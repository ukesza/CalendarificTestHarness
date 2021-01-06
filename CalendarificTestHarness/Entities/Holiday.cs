using System.Collections.Generic;
using Newtonsoft.Json;

namespace CalendarificTestHarness.Entities
{
    public class Holiday
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("description")]
        public string Description;

        [JsonProperty("country")]
        public Country Country;

        [JsonProperty("date")]
        public Date Date;

        [JsonProperty("type")]
        public List<string> Type;

        [JsonProperty("locations")]
        public string Locations;

        [JsonProperty("states")]
        public object States;
    }
}