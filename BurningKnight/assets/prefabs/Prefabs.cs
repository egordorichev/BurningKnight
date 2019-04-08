using System.Collections.Generic;
using Lens.util.file;

namespace BurningKnight.assets.prefabs {
	public static class Prefabs {
		private static Dictionary<string, Prefab> loaded = new Dictionary<string, Prefab>();
		
		public static void Load() {
			Load(FileHandle.FromRoot("Prefabs/"));
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
			
			
		}

		public static void Destroy() {
			loaded.Clear();
		}
	}
}