using System;
using the_depot.Constants;

namespace the_depot.Models
{
    public class DayKey
    {
        public string Key { get; set; }

        public bool Used { get; set; }

        public string UsedOnDate { get; set; }

        public Roles Role { get; set; }
    }
}

