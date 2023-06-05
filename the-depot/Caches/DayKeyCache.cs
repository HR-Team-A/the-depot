using System;
using TheDepot.Models;
using TheDepot.Services;

namespace TheDepot.Caches
{
	public class DayKeyCache
	{
        private static DayKeyCache instance = null;
        public static DayKeyCache Instance
        {
            get
            {
                if (instance == null)
                    instance = new DayKeyCache();
                return instance;
            }
        }

        private DayKeyCache()
        {
            dayKeys = DayKeyService.Load() ?? new List<DayKey>();
        }

        private List<DayKey> dayKeys = new List<DayKey>();
        public List<DayKey> DayKeys
        {
            get
            {
                return dayKeys;
            }
        }

        public void Reload()
        {
            instance = new DayKeyCache();
        }
    }
}

