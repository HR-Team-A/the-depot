using System;
using TheDepot.Caches;
using TheDepot.Models;

namespace TheDepot.Repositories
{
	public class DayKeyRepository
	{
		public static DayKey? Get(int id)
		{
			return DayKeyCache.Instance.DayKeys.FirstOrDefault(x=>x.Id == id);
		}

		public static DayKey? GetByKey(string key)
		{
			return DayKeyCache.Instance.DayKeys.FirstOrDefault(x=>x.Key == key);
		}

        public static List<DayKey> All()
		{
			return DayKeyCache.Instance.DayKeys;
		}
	}
}

