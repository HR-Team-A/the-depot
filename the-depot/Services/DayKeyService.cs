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
        private static string PathEmployees = "../../../KeysEmployees.json";

        /// <summary>
        /// load all daykeys
        /// </summary>
        public static void LoadDayKeys()
        {
            var json = File.ReadAllText(Path);
            List<DayKey> dayKeys = JsonSerializer.Deserialize<List<DayKey>>(json) ?? new List<DayKey>();
            DayKeys = dayKeys;

            json = File.ReadAllText(PathEmployees);
            dayKeys = JsonSerializer.Deserialize<List<DayKey>>(json) ?? new List<DayKey>();
            DayKeys.AddRange(dayKeys);
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

        /// <summary>
        /// set a daykey used
        /// </summary>
        /// <param name="key"></param>
        /// <returns>if operation succeeded</returns>
        public static bool SetDayKeyUsed(string key)
        {
            var daykey = DayKeys.FirstOrDefault(x => x.Key == key);
            if (daykey == null)
                return false;
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

        private static void SaveData()
        {
            string dayKeys = JsonSerializer.Serialize(DayKeys, new JsonSerializerOptions { WriteIndented = true});
            File.WriteAllText(Path, dayKeys);
        }
    }
}
