using System;
using System.Text.Json.Serialization;
using TheDepot.Constants;

namespace TheDepot.Models
{
    public class DayKey
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public Roles Role { get; set; } = Roles.Visitor;
    }
}

