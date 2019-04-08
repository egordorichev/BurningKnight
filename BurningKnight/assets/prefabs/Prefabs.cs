using System.Collections.Generic;
using BurningKnight.save;
using BurningKnight.state;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.assets.prefabs {
	public static class Prefabs {
		private static Dictionary<string, Prefab> loaded = new Dictionary<string, Prefab>();
		
		public static void Load() {
			Load(FileHandle.FromRoot("Prefabs/"));
			Run.Level = null;
		}

		public static Prefab Get(string id) {
			return loaded.TryGetValue(id, out var fab) ? fab : null;
		}

		private static void Load(FileHandle handle) {
			if (!handle.Exists()) {
				return;
			}

			if (handle.IsDirectory()) {
				foreach (var file in handle.ListFileHandles()) {
					Load(file);
				}

				foreach (var file in handle.ListDirectoryHandles()) {
					Load(file);
				}

				return;
			}

			if (handle.Extension != ".lvl") {
				return;
			}
			
			var prefab = new Prefab();
			
			var stream = new FileReader(handle.FullPath);
			SaveManager.ForType(SaveType.Level).Load(prefab.Area, stream);
			stream.Close();

			prefab.Level = Run.Level;
			loaded[handle.NameWithoutExtension] = prefab;
		}

		public static void Destroy() {
			loaded.Clear();
		}
	}
}