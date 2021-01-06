using Newtonsoft.Json;

namespace CalendarificTestHarness.Entities
{
    public class Root
    {
        [JsonProperty("meta")]
        public Meta Meta;

        [JsonProperty("response")]
        public Response Response;
    }
}