using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using the_depot.Models;

namespace the_depot.Services
{
    static public class TourService
    {
        public const string Path = "../../../Tours.json";

        public static List<Tour> LoadTours()
        {
            var json = File.ReadAllText(Path);
            List<Tour> tours = JsonSerializer.Deserialize<List<Tour>>(json) ?? new List<Tour>();
            return tours;
        }
    }
}
