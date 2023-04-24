using System;
namespace the_depot.Models
{
	public class Tour
	{
        public int Id { get; set; }

        public DateTime Start { get; }

        public int MaxAttendees { get; }

        public bool Started { get; }
    }
}

