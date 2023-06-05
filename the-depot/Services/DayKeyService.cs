using System.Runtime.InteropServices;
using System.Text.Json;
using TheDepot.Models;
using TheDepot.Repositories;

namespace TheDepot.Services
{
    static public class DayKeyService
    {
        private static string path = "../../../Keys.json";

        /// <summary>
        /// load all daykeys
        /// </summary>
        public static List<DayKey> Load()
        {
            var json = File.ReadAllText(path);
            List<DayKey> dayKeys = JsonSerializer.Deserialize<List<DayKey>>(json) ?? new List<DayKey>();
            return dayKeys;
        }

        

        ///// <summary>
        ///// get a Daykey
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public static DayKey? GetDayKey(string key)
        //{
        //    return DayKeys.FirstOrDefault(x => x.Key == key);
        //}

        //public static List<DayKey> GetAllKeys()
        //{
        //    return DayKeys;
        //}
    }
}
