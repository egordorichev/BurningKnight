using System;
using System.Collections.Generic;
using System.IO;
using Lens.lightJson;
using Lens.lightJson.Serialization;
using Lens.util;
using Lens.util.file;

namespace Lens.assets {
	public class Locale {
		public static Dictionary<string, string> Map;
		
		public static Dictionary<string, string> Fallback = new Dictionary<string, string>();
		public static Dictionary<string, Dictionary<string, string>> Loaded = new Dictionary<string, Dictionary<string, string>>();
		private static bool LoadedFallback;
		
		public static string Current;
		public static string PrefferedClientLanguage = "en";

		private static void LoadRaw(string name, string path, bool backup = false) {
			if (Loaded.TryGetValue(name, out var cached)) {
				Map = cached;
				return;
			}
			
			var file = FileHandle.FromRoot(path);

			if (!file.Exists()) {
				Log.Error($"Locale {path} was not found!");
				return;
			}

			try {
				var root = JsonValue.Parse(file.ReadAll());
				
				cached = new Dictionary<string, string>();
				Loaded[name] = cached;

				foreach (var entry in root.AsJsonObject) {
					cached[entry.Key] = entry.Value.AsString;
				}

				if (!backup) {
					Loaded[name] = cached;
					Map = cached;
				}
			} catch (Exception e) {
				Log.Error(e);
			}
		}
		
		public static void Load(string locale) {
			Log.Info($"Loading locale {locale}");
			
			if (Current == locale) {
				return;
			}
			
			Current = locale;

			if (!Loaded.ContainsKey(locale)) {
				LoadRaw(locale, $"Locales/{locale}.json");
			}
			
			Map = Loaded[locale];

			if (!LoadedFallback) {
				if (locale != "en") {
					LoadRaw("en", "Locales/en.json", true);
					LoadedFallback = true;
				} else {
					LoadedFallback = true;
					Fallback = Map;
				}
			} 			
		}

		public static void Save() {
			Log.Info($"Saving locale {Current}");
			
			try {
				var file = File.CreateText(FileHandle.FromRoot($"Locales/{Current}.json").FullPath);
				var writer = new JsonWriter(file);
				var root = new JsonObject();

				foreach (var t in Map) {
					root[t.Key] = t.Value;
				}

				writer.Write(root);
				file.Close();
			} catch (Exception e) {
				Log.Error(e);
			}
		}

		public static void Delete() {
			try {
				FileHandle.FromRoot($"Locales/{Current}.json").Delete();
				Current = "en";
			} catch (Exception e) {
				Log.Error(e);
			}
		}

		public static string Get(string key) {
			return Map.ContainsKey(key) ? Map[key] : (Fallback.ContainsKey(key) ? Fallback[key] : key);
		}

		public static bool Contains(string key) {
			return Map.ContainsKey(key) || Fallback.ContainsKey(key);
		}
	}
}