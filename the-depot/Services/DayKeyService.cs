using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using the_depot.Models;

namespace the_depot.Services
{
    static public class DayKeyService
    {
        private static List<DayKey> DayKeys = new List<DayKey>();
        private static string Path = "../../../Keys.json";

        /// <summary>
        /// load all daykeys
        /// </summary>
        public static void LoadDayKeys()
        {
            var json = File.ReadAllText(Path);
            List<DayKey> dayKeys = JsonSerializer.Deserialize<List<DayKey>>(json) ?? new List<DayKey>();
            DayKeys = dayKeys;
        }

        /// <summary>
        /// get a Daykey
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static DayKey? GetDayKey(string key)
        {
            return DayKeys.FirstOrDefault(x => x.Key == key);
        }

        public static List<DayKey> GetAllKeys()
        {
            return DayKeys;
        }

        /// <summary>
        /// set a daykey used
        /// </summary>
        /// <param name="key"></param>
        /// <returns>if operation succeeded</returns>
        public static bool SetDayKeyUsed(string key, out string error)
        {
            error = string.Empty;
            var daykey = DayKeys.FirstOrDefault(x => x.Key == key);
            if (daykey == null)
            {
                error = "De code bestaat niet";
                return false;
            }
            if (daykey.Used)
            {
                var reservations = ReservationService.LoadReservations();
                var reservation = reservations.FirstOrDefault(x => x.Key_Id == daykey.Id);
                var tour = TourService.LoadTours().FirstOrDefault(x => x.Id == reservation.Tour_Id);
                
                // Check if not attended and tour has not started. (If they haven't been to the tour they can reuse the key.)
                if (reservation != null && tour != null && !reservation.Attended && tour.Started)
                {
                    error = $"Deze code is al gebruikt op {daykey.UsedOnDate.ToString("dd-MM-yyyy HH:mm")} om een reservering te maken, annuleer eerst deze reservering om een andere reservering te kunnen plaatsen";
                    return false;
                }
            }
            daykey.Used = true;
            daykey.UsedOnDate = DateTime.Now;
            SaveData();
            return true;
        }

        /// <summary>
        /// cancel reservation 
        /// </summary>
        /// <param name="key"></param>
        /// <returns>if operation succeeded</returns>
        public static bool CancelReservation(string key)
        {
            var daykey = DayKeys.FirstOrDefault(x => x.Key == key);
            if (daykey == null)
                return false;
            daykey.Used = false;
            daykey.UsedOnDate = DateTime.Now;
            SaveData();
            return true;
        }

        /// <summary>
        /// data data to file
        /// </summary>
        private static void SaveData()
        {
            string dayKeys = JsonSerializer.Serialize(DayKeys, new JsonSerializerOptions { WriteIndented = true});
            File.WriteAllText(Path, dayKeys);
        }
    }
}
