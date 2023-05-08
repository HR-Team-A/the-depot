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

            // Load reservation data
            var reservationsJson = File.ReadAllText(ReservationService.Path);
            List<Reservation> reservations = JsonSerializer.Deserialize<List<Reservation>>(reservationsJson) ?? new List<Reservation>();

            // Count attendees and add count to Tour object
            foreach (var tour in tours)
            {
                var tourReservations = reservations.Where(r => r.Tour_Id == tour.Id && r.Attended).ToList();
                tour.Attendees = tourReservations.Count;
            }

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
