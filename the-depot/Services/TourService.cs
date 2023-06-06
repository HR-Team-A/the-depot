using System.Runtime.InteropServices;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using TheDepot.Caches;
using TheDepot.Constants;
using TheDepot.Models;
using TheDepot.Repositories;

namespace TheDepot.Services
{
    static public class TourService
    {
        private const string path = "../../../Tours.json";

        public static List<Tour> Load()
        {
            var json = File.ReadAllText(path);
            List<Tour> tours = JsonSerializer.Deserialize<List<Tour>>(json) ?? new List<Tour>();
            return tours;
        }

        public static void StartTourOrMakeReservation(int tour_Id)
        {
            var tour = TourRepository.Get(tour_Id);
            if (tour == null)
            {
                Menu.WriteTemporaryMessageAndReturnToMenu("Deze rondleiding is helaas verlopen, selecteer een andere rondleiding");
            }
            var code = Menu.WriteMessageAndScanCode($"{tour!.Time.ToString("H:mm")} is geselecteerd. Scan uw code om verder te gaan:");
            var key = DayKeyRepository.GetByKey(code);
            if (key == null || key.Role == Roles.None)
            {
                Menu.WriteTemporaryMessageAndReturnToMenu("Deze code is niet gevonden");
            }
            if(key!.Role == Constants.Roles.DepartmentHead)
            {
                Menu.WriteTemporaryMessageAndReturnToMenu("Dit is een code van het afdelingshoofd, u kunt hier geen reserveringen mee plaatsen.");
            }
            if (key.Role == Roles.Guide)
            {
                StartTour(tour_Id);
            }
            if(key.Role == Roles.Visitor)
            {
                reservation = ReservationRepository.GetByKeyAndTour(key.Id, tour_Id)
                if (reservation == null)
                {
                    MakeReservation(key.Id, tour_Id);
                }
                Menu.WriteTemporaryMessageAndReturnToMenu("U neemt al deel aan deze reservering.");

            }
        }

        public static void StartTour(int tour_Id)
        {
            var tour = TourRepository.Get(tour_Id);
            if (tour == null)
            {
                return;
            }
            tour.Started = true;
            SaveData();
            ReservationService.ScanCodeToAttend(tour, TourService.GetTourStartingInformation(tour.Id) + "\nScan uw code om deel te nemen aan deze rondleiding");
        }

        public static void MakeReservation(int dayKey_Id, int tour_Id)
        {
            ReservationService.AddReservation(dayKey_Id, tour_Id, out string response);
            Menu.WriteTemporaryMessageAndReturnToMenu(response);
        }

        public static string GetTourStartingInformation(int tour_Id)
        {
            var tour = TourRepository.Get(tour_Id);
            var attendees = ReservationRepository.FindByTour(tour_Id);
            var attended = attendees.FindAll(x => x.Attended);
            var attendeesCount = attendees.Count();
            var attendedCount = attended.Count();
            return $"{attendedCount} / {attendeesCount} gescanned, {tour!.MaxAttendees - attendeesCount} extra plekken over.";
        }

        /// <summary>
        /// data data to file
        /// </summary>
        private static void SaveData()
        {
            List<Tour> tours = TourRepository.All();
            string json = JsonSerializer.Serialize(tours, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
            TourRepository.Update();
        }

        public static Option MakeTourListItem(Tour tour)
        {
            var tourTime = tour.Time;
            int tourId = tour.Id;
            int maxAttendees = tour.MaxAttendees;
            int attendees = ReservationRepository.FindByTour(tour.Id).Count();
            int availableSpots = maxAttendees - attendees;
            string text = availableSpots > 0 ? $"{tourTime.ToString("H:mm")} - beschikbare plekken: {availableSpots}" : $"{tourTime.ToString("H:mm")} - Geen beschikbare plekken";
            return new Option(text, () => TourService.StartTourOrMakeReservation(tourId), DateTime.MinValue);
        }

        public static List<Option> MakeToursMenuList()
        {
            // Create options that you want your menu to have
            var optionsReservation = new List<Option>();
            // Loop through all available tours and add them as an option.
            foreach (Tour tour in TourRepository.FindByStartedAndTimeOfDay(false, DateTime.Now.TimeOfDay))
            {
                optionsReservation.Add(MakeTourListItem(tour));
            }
            optionsReservation.Add(new Option("Rondleiding annuleren", () => ReservationService.ScanCodeAndCancelReservation("U heeft geen rondleiding op dit moment."), DateTime.MinValue));
            optionsReservation.Add(new Option("Afdelingshoofd menu", () => AdminService.ScanAdminCode("Scan uw code"), DateTime.MinValue));
            return optionsReservation;
        }
    }
}
