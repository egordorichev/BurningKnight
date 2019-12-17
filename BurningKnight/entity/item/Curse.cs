using System.Collections.Generic;
using Lens.util;

namespace BurningKnight.entity.item {
	public static class Curse {
		private static Dictionary<string, bool> enabled = new Dictionary<string, bool>();
		
		public const string OfUnknown = "bk:of_unknown";
		public const string OfRisk = "bk:of_risk";
		public const string OfCursed = "bk:of_cursed";
		public const string OfBlood = "bk:of_blood";
		public const string OfLost = "bk:of_lost";
		public const string OfKeys = "bk:of_keys";
		public const string OfEgg = "bk:of_egg";

		public static void Enable(string curse) {
			if (IsEnabled(curse)) {
				return;
			}
			
			enabled[curse] = true;
			Log.Info($"Curse {curse} was activated!");
		}

		public static bool IsEnabled(string curse) {
			if (!enabled.ContainsKey(curse)) {
				return false;
			}

			return enabled[curse];
		}
	}
}