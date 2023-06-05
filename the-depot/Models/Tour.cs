using System;
namespace TheDepot.Models
{
	public class Tour
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public int MaxAttendees { get; set; }
        public bool Started { get; set; }
    }
}

