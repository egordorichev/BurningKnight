using System;
using System.Collections.Generic;
using System.Reflection;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.assets.mod {
	public static class Mods {
		public static Dictionary<string, Mod> Loaded = new Dictionary<string, Mod>();
		public static string BurningKnight = "bk";
		
		public static void Load() {
			if (true) {
				return;
			}
			
			var dir = FileHandle.FromRoot("Mods/");

			if (!dir.Exists()) {
				Log.Error("Mod directory was not found, creating and exiting.");
				dir.MakeDirectory();
				return;
			}
			
			Log.Info("Found mod directory");


			
			foreach (var handle in dir.ListDirectoryHandles()) {
				var mod = Mod.Load(handle);

				if (mod == null) {
					Log.Error($"Failed to load mod {handle.Name}");
					continue;
				}

				Log.Info($"Loaded mod {mod.Prefix}");
				Loaded[mod.Prefix] = mod;
			}
		}

		public static void Update(float dt) {
			foreach (var mod in Loaded) {
				
			}
		}

		public static void Render() {
			foreach (var mod in Loaded) {
				
			}
		}

		public static void Destroy() {
			foreach (var mod in Loaded) {
				
			}
		}
	}
}