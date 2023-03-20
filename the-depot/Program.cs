using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading;
using System.Text.Json;
using the_depot.Constants;
using the_depot.Models;

namespace ConsoleApp
{
    class Program
    {
        public static bool TryKey(string key, out DayKey dayKey, out string error)
        {
            error = "";
            var json = File.ReadAllText("../../../Keys.json");
            List<DayKey> dayKeys = JsonSerializer.Deserialize<List<DayKey>>(json);
            dayKey = dayKeys.FirstOrDefault(x => x.Key == key);
            if (dayKey == null)
            {
                error = "Deze code is niet gevonden in ons systeem";
                return false;
            }
            if (dayKey.Used)
            {
                error = $"Deze code is al gebruikt op {DateTime.Parse(dayKey.UsedOnDate).ToString("dd-MM-yyyy HH:mm")} om een reservering te maken, annuleer eerst deze reservering om een andere reservering te kunnen plaatsen";
                return false;
            }
            return true;
        }
    }
}
