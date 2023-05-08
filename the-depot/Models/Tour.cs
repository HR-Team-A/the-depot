using System;
namespace the_depot.Models
{
	public class Tour
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public int MaxAttendees { get; set; }
        public int Attendees { get; set; }
        public bool Started { get; set; }
    }
}

