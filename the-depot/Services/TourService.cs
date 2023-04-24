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

        public static void StartTour(int tour_Id)
        {
            var tours = LoadTours();
            var tour = tours.FirstOrDefault(x => x.Id == tour_Id);
            if (tour == null)
            {
                return;
            }
            tour.Started = true;
            SaveData(tours);
        }

        /// <summary>
        /// data data to file
        /// </summary>
        private static void SaveData(List<Tour> tours)
        {
            string json = JsonSerializer.Serialize(tours, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Path, json);
        }
    }
}
