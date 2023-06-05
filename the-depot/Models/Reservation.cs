using System;
namespace TheDepot.Models
{
	public class Reservation
	{
		public int Key_Id { get; set; }
		public int Tour_Id { get; set; }
		public bool Attended { get; set; }
	}
}

