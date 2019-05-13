using System;
using System.Collections.Generic;
using Lens.lightJson;
using Lens.util;
using Lens.util.file;

namespace Lens.assets {
	public class Locale {
		public static Dictionary<string, string> Map;
		
		private static Dictionary<string, string> fallback = new Dictionary<string, string>();
		private static Dictionary<string, Dictionary<string, string>> loaded = new Dictionary<string, Dictionary<string, string>>();
		private static bool loadedFallback;
		
		public static string Current;

		private static void LoadRaw(string name, string path, bool backup = false) {
			if (loaded.TryGetValue(name, out var cached)) {
				Map = cached;
				return;
			}
			
			var file = new FileHandle(path);

			if (!file.Exists()) {
				Log.Error($"Locale {path} was not found!");
				return;
			}

			try {
				var root = JsonValue.Parse(file.ReadAll());
				
				cached = new Dictionary<string, string>();
				loaded[name] = cached;

				foreach (var entry in root.AsJsonObject) {
					cached[entry.Key] = entry.Value.AsString;
				}

				if (!backup) {
					loaded[name] = cached;
					Map = cached;
				}
			} catch (Exception e) {
				Log.Error(e);
			}
		}
		
		public static void Load(string locale) {
			if (Current == locale) {
				return;
			}
			
			Current = locale;
			LoadRaw(locale, $"Content/Locales/{locale}.json");

			if (!loadedFallback && locale != "en") {
				LoadRaw("en", "Content/Locales/en.json", true);
				loadedFallback = true;
			} 			
		}

		public static string Get(string key) {
			return Map.ContainsKey(key) ? Map[key] : (fallback.ContainsKey(key) ? fallback[key] : key);
		}

		public static bool Contains(string key) {
			return Map.ContainsKey(key) || fallback.ContainsKey(key);
		}
	}
}