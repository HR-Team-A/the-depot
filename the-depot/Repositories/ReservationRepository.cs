using System;
using TheDepot.Caches;
using TheDepot.Models;

namespace TheDepot.Repositories
{
	public class ReservationRepository
	{
        public static List<Reservation> FindByTour(int tour_Id)
        {
			return ReservationCache.Instance.Reservations.FindAll(x=>x.Tour_Id == tour_Id);
        }

        public static List<Reservation> All()
        {
            return ReservationCache.Instance.Reservations;
        }

        public static Reservation? Get(int id)
        {
            return ReservationCache.Instance.Reservations.FirstOrDefault(x=>x.Key_Id == id);
        }

        public static Reservation? GetByKeyAndTour(int dayKey_Id, int tour_Id)
        {
            return ReservationCache.Instance.Reservations.FirstOrDefault(x => x.Key_Id == dayKey_Id && x.Tour_Id == tour_Id);
        }

        public static void Update()
        {
            ReservationCache.Instance.Reload();
        }
    }
}

