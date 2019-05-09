using System.Collections.Generic;
using Lens.lightJson;
using Lens.util;
using Lens.util.file;

namespace Lens.assets {
	public class Locale {
		private static Dictionary<string, string> map = new Dictionary<string, string>();
		private static Dictionary<string, string> fallback = new Dictionary<string, string>();
		private static bool loadedFallback;
		
		public static string Current;

		private static void LoadRaw(string path, bool backup = false) {
			var file = new FileHandle(path);

			if (!file.Exists()) {
				Log.Error($"Locale {path} was not found!");
				return;
			}
			
			var root = JsonValue.Parse(file.ReadAll());

			foreach (var entry in root.AsJsonObject) {
				if (backup) {
					fallback[entry.Key] = entry.Value.AsString;
				} else {
					map[entry.Key] = entry.Value.AsString;
				}
			}
		}
		
		public static void Load(string locale) {
			Current = locale;
			map.Clear();

			LoadRaw($"Content/Locales/{locale}.json");

			if (!loadedFallback && locale != "en") {
				LoadRaw("Content/Locales/en.json", true);
				loadedFallback = true;
			} 			
		}

		public static string Get(string key) {
			return map.ContainsKey(key) ? map[key] : (fallback.ContainsKey(key) ? fallback[key] : key);
		}

		public static bool Contains(string key) {
			return map.ContainsKey(key) || fallback.ContainsKey(key);
		}
	}
}