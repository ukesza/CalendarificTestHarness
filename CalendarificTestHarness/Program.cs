using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using CalendarificTestHarness.Entities;
using Newtonsoft.Json;

namespace CalendarificTestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            // You do really need to change this.
            var key = "xxYourKeyHerexx";

            var ukUrl = BuildUrl(key, "2021", "uk", "gb-eng", "local,national");
            var maltaUrl = BuildUrl(key, "2021", "mt", null, "local,national");
            var barcelonaUrl = BuildUrl(key, "2021", "es", "es-ct");

            var client = new HttpClient();
            client.BaseAddress = new Uri("https://calendarific.com");

            var ukHolidays = GetHolidays(client, ukUrl);
            var maltaHolidays = GetHolidays(client, maltaUrl);
            var barcelonaHolidays = GetHolidays(client, barcelonaUrl);

            ManageBarcelonaHolidays(barcelonaHolidays);
            RemoveWeekendHolidays(ukHolidays);

            var mergedHolidays = new List<Holiday>();
            mergedHolidays.AddRange(maltaHolidays);
            mergedHolidays.AddRange(ukHolidays);

            foreach (var barcelonaHoliday in barcelonaHolidays)
            {
                if (ValidHoliday(barcelonaHoliday))
                {
                    mergedHolidays.Add(barcelonaHoliday);
                }
            }

            RemoveWeekendHolidays(mergedHolidays);

            var groupedHolidays = mergedHolidays.OrderBy(h => h.Date.DateTime).GroupBy(h => h.Date.DateTime);

            foreach (var groupedHoliday in groupedHolidays)
            {
                Console.WriteLine($"Date: {groupedHoliday.Key}");
                Console.WriteLine($"Name: {groupedHoliday.First().Name}");
                Console.Write("Locations: ");

                foreach (var holiday in groupedHoliday)
                {
                    Console.Write($"{holiday.Country.Name}. ");
                }

                Console.WriteLine();
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Constructs the URI for the request from the passed parameters, many of which are optional
        /// </summary>
        /// <param name="key">Your API Key</param>
        /// <param name="year">The year you want the data for</param>
        /// <param name="country">The ISO country code of the country</param>
        /// <param name="location">The province/region/state appended to the ISO country code with a dash. gb-eng for England</param>
        /// <param name="type">The comma separated list of holiday types</param>
        /// <returns></returns>
        private static string BuildUrl(string key, string year, string country, string location = null, string type = null)
        {
            var builder = new StringBuilder($"/api/v2/holidays?&api_key={key}&country={country}");

            if (!string.IsNullOrEmpty(location))
            {
                builder.Append($"&location={location}");
            }

            if (!string.IsNullOrEmpty(type))
            {
                builder.Append($"&type={type}");
            }

            builder.Append($"&year={year}");

            return builder.ToString();
        }

        private static List<Holiday> GetHolidays(HttpClient client, string url)
        {
            var task = client.GetAsync(url);
            var result = task.Result;
            result.EnsureSuccessStatusCode();
            var content = result.Content.ReadAsStringAsync().Result;
            var root = JsonConvert.DeserializeObject<Root>(content);

            return root.Response.Holidays;
        }

        /// <summary>
        /// Custom code to align the data which comes back for Spain/Catalonia with the official list of holidays as per the Barcelona Municipality
        /// </summary>
        /// <param name="holidays"></param>
        private static void ManageBarcelonaHolidays(List<Holiday> holidays)
        {
            // This is the official source of all Barcelona holidays
            // https://ajuntament.barcelona.cat/calendarifestius/en/

            // In Catalonia the holiday is held on the Monday, but reflects in the data on a Sunday
            var whitSunday = holidays.FirstOrDefault(h => h.Name == "Whit Sunday/Pentecost");
            if (whitSunday != null)
            {
                whitSunday.Type.Clear();
                whitSunday.Type.Add("Barcelona holiday");
                whitSunday.Name = "Whit Monday/Pentecost Monday";
                whitSunday.Date.DateTime = whitSunday.Date.DateTime.AddDays(1);
            }

            // Local Barcelona holiday, held on the 24th of September every year. 
            var laMerce = new Holiday()
            {
                Name = "La Mercè",
                Description = "The annual celebration of the city of Barcelona, celebrating the Virgin of Grace.",
                Type = new List<string> { "Barcelona holiday" },
                Country = new Country
                {
                    Id = "es",
                    Name = "Spain"
                },
                Date = new Date
                {
                    DateTime = new DateTime(whitSunday.Date.DateTime.Year, 9, 24)
                }
            };

            // This holiday doesnt appear in the data at all for some strange reason
            var allSaints = new Holiday()
            {
                Name = "All Saints Day",
                //Description = "The annual celebration of the city of Barcelona, celebrating the Virgin of Grace.",
                Type = new List<string> { "National holiday" },
                Country = new Country
                {
                    Id = "es",
                    Name = "Spain"
                },
                Date = new Date
                {
                    DateTime = new DateTime(whitSunday.Date.DateTime.Year, 11, 1)
                }
            };

            holidays.Add(laMerce);
            holidays.Add(allSaints);

            // This holiday appears to be a spanish national holiday, but isnt being celebrated in Barcelona
            var feastOfTheAssumption = holidays.FirstOrDefault(h => h.Name == "Assumption of Mary");
            if (feastOfTheAssumption != null)
            {
                holidays.Remove(feastOfTheAssumption);
            }

            var assumptionObserved = holidays.FirstOrDefault(h => h.Name == "Assumption observed");
            if (assumptionObserved != null)
            {
                holidays.Remove(assumptionObserved);
            }
        }

        /// <summary>
        /// For the purposes of the author, there was no need for holidays which fell on a weekend, so these were removed.
        /// </summary>
        /// <param name="holidays"></param>
        private static void RemoveWeekendHolidays(List<Holiday> holidays)
        {
            var weekendDays = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday };
            var weekendHolidays = holidays.Where(h => weekendDays.Contains(h.Date.DateTime.DayOfWeek)).ToList();

            foreach (var weekendHoliday in weekendHolidays)
            {
                holidays.Remove(weekendHoliday);
            }
        }

        /// <summary>
        /// For the UK and Malta, the unnecessary holiday types were filtered in the request.
        /// Due to teh shenanigans with the Spain/Catalonia/Barcelona holidays this had to be done here manually.
        /// Unlikely to be needed for many other use cases.
        /// </summary>
        /// <param name="holiday"></param>
        /// <returns></returns>
        private static bool ValidHoliday(Holiday holiday)
        {
            if (holiday.Country.Name == "Spain")
            {
                var holidayType = holiday.Type.First();

                switch (holidayType)
                {
                    case "National holiday":
                    case "Local holiday":
                    case "Barcelona holiday":
                    {
                        return true;
                    }
                    default:
                    {
                        return false;
                    }
                }
            }

            if (holiday.Country.Name == "United Kingdom")
            {
                return true;
            }

            if (holiday.Country.Name == "Malta")
            {
                return true;
            }

            return false;
        }
    }
}
