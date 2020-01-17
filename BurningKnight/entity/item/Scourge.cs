using System.Collections.Generic;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.entity.item {
	public static class Scourge {
		private static Dictionary<string, bool> enabled = new Dictionary<string, bool>();
		public static List<string> Defined = new List<string>();
		
		public const string OfUnknown = "bk:of_unknown";
		public const string OfRisk = "bk:of_risk";
		public const string OfScourged = "bk:of_scourged";
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

		static Scourge() {
			Define(OfUnknown);
			Define(OfRisk);
			Define(OfScourged);
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
			Log.Info($"Scourge {curse} was activated!");
		}

		public static void Disable(string curse) {
			if (!IsEnabled(curse)) {
				return;
			}
			
			enabled[curse] = false;
			Log.Info($"Scourge {curse} was deactivated!");
		}

		public static bool IsEnabled(string curse) {
			if (!enabled.ContainsKey(curse)) {
				return false;
			}

			return enabled[curse];
		}

		public static bool ShouldAppear(string id) {
			if (id == OfDeath) {
				foreach (var s in Defined) {
					if (s != OfDeath && !IsEnabled(s)) {
						return false;
					}
				}
			} else if (id == OfScourged) {
				var found = false;
				
				foreach (var s in Defined) {
					if (IsEnabled(s)) {
						found = true;
						break;
					}
				}

				if (!found) {
					return false;
				}
			}
			
			return !IsEnabled(id);
		}

		public static string Generate() {
			var list = GenerateList();
			return list.Count == 0 ? null : list[Rnd.Int(list.Count)];
		}

		public static string GenerateItemId() {
			var id = Generate();

			if (id == null) {
				return null;
			}

			return $"bk:scourge_{id.Replace("bk:", "")}";
		}
		
		public static List<string> GenerateList() {
			var list = new List<string>();

			foreach (var s in Defined) {
				if (ShouldAppear(s)) {
					list.Add(s);
				}
			}

			return list;
		}
	}
}