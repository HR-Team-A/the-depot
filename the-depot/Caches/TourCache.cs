using System;
using TheDepot.Models;
using TheDepot.Services;

namespace TheDepot.Caches
{
	public class TourCache
	{
        private static TourCache? instance = null;
        public static TourCache Instance
        {
            get
            {
                if (instance == null)
                    instance = new TourCache();
                return instance;
            }
        }

        private TourCache()
        {
            tours = TourService.Load() ?? new List<Tour>();
        }

        private List<Tour> tours = new List<Tour>();
        public List<Tour> Tours
        {
            get
            {
                return tours;
            }
        }

        public void Reload()
        {
            instance = new TourCache();
        }
    }
}

