using System.Text.Json;
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
    }
}
