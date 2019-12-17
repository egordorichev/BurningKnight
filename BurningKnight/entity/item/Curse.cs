using System.Collections.Generic;
using Lens.util;

namespace BurningKnight.entity.item {
	public static class Curse {
		private static Dictionary<string, bool> enabled = new Dictionary<string, bool>();
		public static List<string> Defined = new List<string>();
		
		public const string OfUnknown = "bk:of_unknown";
		public const string OfRisk = "bk:of_risk";
		public const string OfCursed = "bk:of_cursed";
		public const string OfBlood = "bk:of_blood";
		public const string OfLost = "bk:of_lost";
		public const string OfKeys = "bk:of_keys";
		public const string OfEgg = "bk:of_egg";
		public const string OfIllness = "bk:of_illness";
		public const string OfGreed = "bk:of_greed";
		
		public const string OfDeath = "bk:of_death";

		public static void Define(string curse) {
			Defined.Add(curse);
		}

		static Curse() {
			Define(OfUnknown);
			Define(OfRisk);
			Define(OfCursed);
			Define(OfBlood);
			Define(OfLost);
			Define(OfKeys);
			Define(OfEgg);
			Define(OfIllness);
			Define(OfGreed);
			Define(OfDeath);
		}

		public static void Clear() {
			enabled.Clear();
		}

		public static void Enable(string curse) {
			if (IsEnabled(curse)) {
				return;
			}
			
			enabled[curse] = true;
			Log.Info($"Curse {curse} was activated!");
		}

		public static void Disable(string curse) {
			if (!IsEnabled(curse)) {
				return;
			}
			
			enabled[curse] = false;
			Log.Info($"Curse {curse} was deactivated!");
		}

		public static bool IsEnabled(string curse) {
			if (!enabled.ContainsKey(curse)) {
				return false;
			}

			return enabled[curse];
		}
	}
}