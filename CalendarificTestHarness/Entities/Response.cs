using System.Collections.Generic;
using Newtonsoft.Json;

namespace CalendarificTestHarness.Entities
{
    public class Response
    {
        [JsonProperty("holidays")]
        public List<Holiday> Holidays;
    }
}