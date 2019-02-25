using System.Collections.Generic;

namespace Lens.assets {
	public class Locale {
		private static Dictionary<string, string> Map = new Dictionary<string, string>();
		private static Dictionary<string, string> Fallback = new Dictionary<string, string>();
		public static string Current;

		private static void LoadRaw(string json, bool fallback = false) {
			
		}

		public static void Load(string locale) {
			Current = locale;
			Map.Clear();

			LoadRaw(Gdx.Files.Internal($"locales/{locale}.json")).ReadString(), false);

			if (!Locale.Equals("en")) {
				LoadRaw(Gdx.Files.Internal("locales/en.json").ReadString(), true);
			} 
		}

		public static boolean Has(string Name) {
			return Map.ContainsKey(Name) || Fallback.ContainsKey(Name);
		}

		public static string Get(string Name) {
			return Map.ContainsKey(Name) ? Map.Get(Name) : Fallback.GetOrDefault(Name, Name);
		}
	}
}