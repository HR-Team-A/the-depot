using System;
using System.Text.Json;
using the_depot.Models;

namespace the_depot.Services
{
	public class ReservationService
	{
        private const string Path = "../../../Reservations.json";

        /// <summary>
        /// load all daykeys
        /// </summary>
        public static List<Reservation> LoadReservations()
        { 
            var json = File.ReadAllText(Path);
            List<Reservation> reservations = JsonSerializer.Deserialize<List<Reservation>>(json) ?? new List<Reservation>();
            return reservations;
        }

        public static string SetReservationAttended(int dayKey_Id, int tour_Id)
        {
            var reservations = LoadReservations();
            var reservation = reservations.FirstOrDefault(x => x.Key_Id == dayKey_Id);
            if(reservation == null)
            {
                return "Er is geen reservering met uw sleutel";
            }
            reservation.Attended = true;
            SaveData(reservations);
            return "";
        }

        public static string AddReservation(int dayKey_Id, int tour_Id)
        {
            var reservations = LoadReservations();
            if(reservations.Any(x => x.Key_Id == dayKey_Id))
            {
                return "Er is al een reservering gemaakt, annuleer deze eerst om een nieuwe aan te maken.";
            }
            reservations.Add(new Reservation { Attended = false, Key_Id = dayKey_Id, Tour_Id = tour_Id }) ;
            SaveData(reservations);
            return string.Empty;
        }

        public static void CancelReservation(int dayKey_Id)
        {
            var reservations = LoadReservations();
            var reservation = reservations.FirstOrDefault(x => x.Key_Id == dayKey_Id);
            if(reservation != null) 
                reservations.Remove(reservation);
            SaveData(reservations);
        }

        /// <summary>
        /// data data to file
        /// </summary>
        private static void SaveData(List<Reservation> reservations)
        {
            string json = JsonSerializer.Serialize(reservations, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Path, json);
        }
    }
}

