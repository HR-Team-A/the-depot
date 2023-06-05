using System;
using TheDepot.Models;
using TheDepot.Services;

namespace TheDepot.Caches
{
	public class ReservationCache
	{
        private static ReservationCache? instance = null;
        public static ReservationCache Instance
        {
            get
            {
                if (instance == null)
                    instance = new ReservationCache();
                return instance;
            }
        }

        private ReservationCache()
        {
            reservations = ReservationService.Load() ?? new List<Reservation>();
        }

        private List<Reservation> reservations = new List<Reservation>();
        public List<Reservation> Reservations
        {
            get
            {
                return reservations;
            }
        }

        public void Reload()
        {
            instance = new ReservationCache();
        }
    }
}

