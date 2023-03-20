using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading;
using System.Text.Json;

namespace ConsoleApp
{
    public class DayKey
    {
        public string Key { get; set; }

        public bool Used { get; set; }

        public string UsedOnDate { get; set; }
    }

    class Program
    {

        public static bool TryKey(string key, out string error)
        {
            error = "";
            var json = File.ReadAllText("../../../Keys.json");
            List<DayKey> dayKeys = JsonSerializer.Deserialize<List<DayKey>>(json);
            var validKey = dayKeys.FirstOrDefault(x => x.Key == key);
            if (validKey == null)
            {
                error = "Deze code is niet gevonden in ons systeem";
                return false;
            }
            if (validKey.Used)
            {
                error = $"Deze code is al gebruikt op {DateTime.Parse(validKey.UsedOnDate).ToString("dd-MM-yyyy HH:mm")} om een reservering te maken, annuleer eerst deze reservering om een andere reservering te kunnen plaatsen";
                return false;
            }
            return true;
        }
    }
}
