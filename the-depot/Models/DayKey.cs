using System;
using System.Text.Json.Serialization;
using the_depot.Constants;

namespace the_depot.Models
{
    public class DayKey
    {
        public string Key { get; set; } = string.Empty;

        public bool Used { get; set; }

        public DateTime UsedOnDate { get; set; }

        public Roles Role { get; set; } = Roles.Visitor;
    }
}

