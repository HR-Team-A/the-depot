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

        public static Tour? GetTour(int tourId)
        {
            var tours = LoadTours();
            return tours.FirstOrDefault(x => x.Id == tourId);
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

        public static string GetTourStartingInformation(int tour_id)
        {
            var tour = GetTour(tour_id);
            var attendeesCount = GetAttendeesCount(tour_id);
            var attendedCount = GetAttendedCount(tour_id);
            return $"{attendedCount} / {attendeesCount} gescanned, {tour!.MaxAttendees - attendeesCount} extra plekken over.";
        }

        /// <summary>
        /// data data to file
        /// </summary>
        private static void SaveData(List<Tour> tours)
        {
            string json = JsonSerializer.Serialize(tours, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Path, json);
        }
        
        /// <summary>
        /// get attendees count of a tour
        /// </summary>
        /// <param name="tour_Id"></param>
        public static int GetAttendeesCount(int tour_Id)
        {
            var reservations = ReservationService.LoadReservations();
            var tourReservations = reservations.Where(r => r.Tour_Id == tour_Id).ToList();
            
            return tourReservations.Count;
        }


        /// <summary>
        /// get attendees count of a tour
        /// </summary>
        /// <param name="tour_Id"></param>
        public static int GetAttendedCount(int tour_Id)
        {
            var reservations = ReservationService.LoadReservations();
            var tourReservations = reservations.Where(r => r.Tour_Id == tour_Id && r.Attended).ToList();

            return tourReservations.Count;
        }
    }
}
