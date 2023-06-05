using System;
using TheDepot.Caches;
using TheDepot.Models;

namespace TheDepot.Repositories
{
	public class TourRepository
	{
        public static List<Tour> All()
        {
            return TourCache.Instance.Tours;
        }

        public static Tour? Get(int id)
        {
            return TourCache.Instance.Tours.FirstOrDefault(x => x.Id == id);
        }

        public static void Update() {
            TourCache.Instance.Reload();
        }

        public static List<Tour> FindByStarted(bool started)
        {
            return TourCache.Instance.Tours.FindAll(x=>x.Started);
        }

        public static List<Tour> FindByStartedAndTimeOfDay(bool started, TimeSpan timeOfDay)
        {
            return TourCache.Instance.Tours.FindAll(x=>x.Started == started && x.Time.TimeOfDay >= timeOfDay);
        }
    }
}

