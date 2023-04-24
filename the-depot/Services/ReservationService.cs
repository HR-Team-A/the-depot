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

        public static void SetReservationAttended(int dayKey_Id)
        {
            DayKeyService.LoadDayKeys();
            var reservations = LoadReservations();
            var reservation = reservations.FirstOrDefault(x => x.Key_Id == dayKey_Id);
            if(reservation == null)
            {
                return;
            }
            reservation.Attended = true;
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

