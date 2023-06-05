using System;
using System.Runtime.InteropServices;
using System.Text.Json;
using TheDepot.Models;
using TheDepot.Repositories;

namespace TheDepot.Services
{
    public class ReservationService
    {
        private const string Path = "../../../Reservations.json";

        /// <summary>
        /// load all daykeys
        /// </summary>
        public static List<Reservation> Load()
        {
            var json = File.ReadAllText(Path);
            List<Reservation> reservations = JsonSerializer.Deserialize<List<Reservation>>(json) ?? new List<Reservation>();
            return reservations;
        }

        public static bool SetReservationAttended(int dayKey_Id, int tour_Id, out string error)
        {
            error = "";
            var tour = TourRepository.Get(tour_Id);
            var reservations = ReservationRepository.FindByTour(tour_Id);
            if (tour == null)
            {
                error = "Deze rondleiding is verlopen";
                return false;
            }
            var reservation = reservations.FirstOrDefault(x=>x.Key_Id == dayKey_Id);
            if (reservation == null)
            {
                error = "U heeft geen reservering voor deze rondleiding";
                return true;
            }
            reservation.Attended = true;
            SaveData();
            return true;
        }

        public static void ScanCodeToAttend(Tour tour, string message)
        {
            var code = Menu.WriteMessageAndScanCode(message);
            var dayKey = DayKeyRepository.GetByKey(code);
            if (code == "stop")
            {
                Menu.ChooseMenu(TourService.MakeToursMenuList());
            }
            if (dayKey == null)
            {
                ScanCodeToAttend(tour, $"{TourService.GetTourStartingInformation(tour.Id)}  \nDeze sleutel is niet gevonden, probeer het opnieuw");
            }
            if(dayKey!.Role != Constants.Roles.Visitor)
            {
                ScanCodeToAttend(tour, $"{TourService.GetTourStartingInformation(tour.Id)}  \nDeze sleutel heeft verkeerde rechten");
            }
            var succeeded = ReservationService.SetReservationAttended(dayKey!.Id, tour.Id, out string error);
            if (!succeeded)
            {
                ScanCodeToAttend(tour, $"{TourService.GetTourStartingInformation(tour.Id)}  \n" + error);
            }
            // We check the OS, only windows supports the useage of Console.Beep();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.Beep();
            }
            else
            {
                Console.WriteLine("Biep boop");
            }
            ScanCodeToAttend(tour, $"{TourService.GetTourStartingInformation(tour.Id)}  \nU bent successvol aangemeld, laat de volgende bezoeker hun code scannen");

        }

        public static Reservation? AddNotReservedAttended(int dayKey_Id, int tour_Id, out string response)
        {
            var tour = TourRepository.Get(tour_Id);
            var attendeesCount = ReservationRepository.FindByTour(tour_Id).Count();
            if (tour == null)
            {
                response = "Deze rondleiding is verlopen";
                return null;
            }
            if (tour.MaxAttendees <= attendeesCount)
            {
                response = "U heeft geen reservering voor deze rondleiding, en de rondleiding is vol";
                return null;
            }
            var reservation = AddReservation(dayKey_Id, tour_Id, out string error);
            response = error;
            if (reservation == null)
            {
                return reservation;
            }
            reservation.Attended = true;
            SaveData();
            return reservation;
        }

        public static Reservation? AddReservation(int dayKey_Id, int tour_Id, out string addResponse)
        {
            addResponse = "";
            var cancelled = CancelReservation(dayKey_Id, out string cancelResponse);
            addResponse = cancelResponse;
            if (!cancelled)
            {
                return null;
            }
            var reservations = ReservationRepository.All();
            var reservation = new Reservation { Attended = false, Key_Id = dayKey_Id, Tour_Id = tour_Id };
            reservations.Add(reservation);
            SaveData();
            return reservation;
        }

        public static bool CancelReservation(int dayKey_Id, out string cancelResponse)
        {
            cancelResponse = string.Empty;
            var reservations = ReservationRepository.All();
            var reservation = reservations.FirstOrDefault(x => x.Key_Id == dayKey_Id);
            var tour = TourRepository.Get(reservation.Tour_Id);
            if(reservation == null)
            {
                return true;
            }
            if (reservation.Attended)
            {
                cancelResponse = "U heeft al deelgenomen aan een rondleiding, u kunt deze niet annuleren.";
                return false;
            }
            string tourTime = tour!.Time.ToString("H:mm");
            cancelResponse = "De reservering van " + tourTime + " is succesvol vervangen.";
            reservations.Remove(reservation);
            SaveData();
            return true;
        }

        //cancel the reservation 
        public static void ScanCodeAndCancelReservation(string message)
        {
            Console.Clear();
            Console.WriteLine("Scan code:");
            var code = Console.ReadLine() ?? string.Empty;
            var dayKey = DayKeyRepository.GetByKey(code);
            if (dayKey == null)
            {
                Menu.WriteTemporaryMessageAndReturnToMenu("Deze code is niet gevonden.");
            }
            if (dayKey.Role == Constants.Roles.Visitor)
            {
                Menu.WriteTemporaryMessageAndReturnToMenu("Deze code is niet geldig.");
            }
            var id = dayKey.Id;

            ReservationService.CancelReservation((int)id, out string error);
            if (!string.IsNullOrEmpty(error))
            {
                Menu.WriteTemporaryMessageAndReturnToMenu(error);
            }
            Menu.WriteTemporaryMessageAndReturnToMenu(message);
        }

        /// <summary>
        /// data data to file
        /// </summary>
        private static void SaveData()
        {
            var reservations = ReservationRepository.All();
            string json = JsonSerializer.Serialize(reservations, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Path, json);
            ReservationRepository.Update();
        }
    }
}

