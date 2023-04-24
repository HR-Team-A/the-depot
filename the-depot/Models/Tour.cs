using System;
using the_depot.Constants;

namespace the_depot.Models
{
    public class Tour
	{
        public DateTime Time { get; set; }
        public int MaxAttendees { get; set; }
        public bool Started { get; set; }
    }
}

