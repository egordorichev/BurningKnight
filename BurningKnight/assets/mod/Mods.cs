using System;
using System.Collections.Generic;
using System.Reflection;
using Lens.assets;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.assets.mod {
	public static class Mods {
		public static Dictionary<string, Mod> Loaded = new Dictionary<string, Mod>();
		public static string BurningKnight = "bk";
		
		public static void Load() {
			if (!Assets.LoadMods) {
				return;
			}
			
			var dir = FileHandle.FromRoot("Mods/");

			if (!dir.Exists()) {
				Log.Error("Mod directory was not found, creating and exiting.");
				dir.MakeDirectory();
				return;
			}
			
			Log.Info("Found mod directory");

			foreach (var handle in dir.ListFileHandles()) {
				if (handle.Extension != ".dll") {
					continue;
				}
				
				var mod = Mod.Load(handle);

				if (mod == null) {
					Log.Error($"Failed to load mod {handle.Name}");
					continue;
				}

				var prefix = mod.Prefix;

				if (Loaded.ContainsKey(prefix)) {
					Log.Error($"Conflicting mods with the same prefix {prefix}");
					continue;
				}
				
				Log.Info($"Loaded mod {prefix}");
				Loaded[prefix] = mod;
				mod.Init();
			}
		}

		public static void Update(float dt) {
			foreach (var mod in Loaded.Values) {
				mod.Update(dt);
			}
		}

		public static void Render() {
			foreach (var mod in Loaded.Values) {
				mod.Render();
			}
		}

		public static void Destroy() {
			foreach (var mod in Loaded.Values) {
				mod.Destroy();
			}
		}
	}
}