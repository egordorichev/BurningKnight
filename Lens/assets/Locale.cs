using System;
using System.Collections.Generic;
using Lens.util.file;
using Newtonsoft.Json;

namespace Lens.assets {
	public class Locale {
		private static Dictionary<string, string> map = new Dictionary<string, string>();
		private static Dictionary<string, string> fallback = new Dictionary<string, string>();
		private static bool loadedFallback;
		
		public static string Current;

		private static void LoadRaw(string path, bool fallback = false) {
			var file = new FileHandle(path);
			var deserialized = JsonConvert.DeserializeObject<Dictionary<string, string>>(file.ReadAll());

			foreach (string name in deserialized.Values) {
				Console.WriteLine(name);
			}
			
			// todo
		}
		
		public static void Load(string locale) {
			Current = locale;
			map.Clear();

			LoadRaw($"Locales/{locale}.json");

			if (!loadedFallback && locale != "en") {
				LoadRaw("Locales/en.json", true);
				loadedFallback = true;
			} 
		}

		public static string Get(string key) {
			return map.ContainsKey(key) ? map[key] : fallback[key];
		}
	}
}