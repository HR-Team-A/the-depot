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
                reservation = AddNotReservedAttended(dayKey_Id, tour_Id, out string response);
                if(reservation == null)
                {
                    error = response;
                    return true;
                }
            }
            reservation.Attended = true;
            SaveData();
            return true;
        }

        public static void ScanCodeToAttend(Tour tour, string message)
        {
            var code = Menu.WriteMessageAndScanCode(message);
            var dayKey = DayKeyRepository.GetByKey(code);
            if (code == "start")
            {
                Menu.ChooseMenu(TourService.MakeToursMenuList());
            }
            if (dayKey == null)
            {
                ScanCodeToAttend(tour, $"{TourService.GetTourStartingInformation(tour.Id)}  \nDeze code is niet gevonden, probeer het opnieuw");
            }
            if(dayKey!.Role != Constants.Roles.Visitor)
            {
                ScanCodeToAttend(tour, $"{TourService.GetTourStartingInformation(tour.Id)}  \nDeze code heeft verkeerde rechten");
            }
            var succeeded = SetReservationAttended(dayKey!.Id, tour.Id, out string error);
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
            var tour = TourRepository.Get(tour_Id);
            var attendeesCount = ReservationRepository.FindByTour(tour_Id).Count();
            var reservations = ReservationRepository.All();
            var old_reservation = reservations.FirstOrDefault(x => x.Key_Id == dayKey_Id);
            addResponse = "";

            if (old_reservation != null && tour_Id == old_reservation!.Tour_Id)
            {
                addResponse = "U heeft al een reservering voor deze rondleiding.";
                return null;
            }

            var cancelled = CancelReservation(dayKey_Id, out string cancelResponse);
            addResponse = cancelResponse;

            if (tour!.MaxAttendees <= attendeesCount)
            {
                addResponse = "U heeft geen reservering voor deze rondleiding, en de rondleiding is vol";
                return null;
            }
            if (!cancelled)
            {
                return null;
            }
            var reservation = new Reservation { Attended = false, Key_Id = dayKey_Id, Tour_Id = tour_Id };
            reservations.Add(reservation);
            SaveData();
            addResponse += $"U heeft succesvol een reservering geplaatst op: {tour!.Time.ToString("HH:mm")}.";
            return reservation;
        }

        public static bool CancelReservation(int dayKey_Id, out string cancelResponse)
        {
            cancelResponse = string.Empty;
            var reservations = ReservationRepository.All();
            var reservation = reservations.FirstOrDefault(x => x.Key_Id == dayKey_Id);
            if(reservation == null)
            {
                return true;
            }
            if (reservation.Attended)
            {
                cancelResponse = "U heeft al deelgenomen aan een rondleiding, u kunt deze niet annuleren.";
                return false;
            }
            var tour = TourRepository.Get(reservation.Tour_Id);
            var attendeesCount = ReservationRepository.FindByTour(reservation.Tour_Id).Count();
            if (tour != null)
            {
                if (tour.MaxAttendees <= attendeesCount)
                {
                    cancelResponse = "U heeft geen reservering voor deze rondleiding, en de rondleiding is vol";
                    return false;
                }
                else
                {
                string tourTime = tour!.Time.ToString("H:mm");
                cancelResponse = "De reservering van " + tourTime + " is succesvol vervangen. ";
                }
            }
            else
            {
                cancelResponse = "Uw vorige reservering is succesvol vervangen";
            }
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
            if (dayKey!.Role != Constants.Roles.Visitor)
            {
                Menu.WriteTemporaryMessageAndReturnToMenu("Deze code is niet geldig.");
            }
            var id = dayKey.Id;
            var reservations = ReservationRepository.All();
            var reservation = reservations.FirstOrDefault(x => x.Key_Id == id);

            if (reservation != null)
            {
                var tour = TourRepository.Get(reservation!.Tour_Id);

                string tourTime = tour!.Time.ToString("H:mm");
                message = "De reservering van " + tourTime + " is succesvol geannuleerd.";
            }
                var cancelled = CancelReservation(id, out string error);

            if (!cancelled)
            {
                Menu.WriteTemporaryMessageAndReturnToMenu(error);
            }

            Menu.WriteTemporaryMessageAndReturnToMenu(message);
        }

        /// <summary>
        /// save data to file
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

