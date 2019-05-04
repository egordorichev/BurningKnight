using System.Collections.Generic;
using System.IO;
using BurningKnight.save;
using BurningKnight.state;
using Lens;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.assets.prefabs {
	public static class Prefabs {
		private static Dictionary<string, Prefab> loaded = new Dictionary<string, Prefab>();
		private static List<string> paths = new List<string>();
		
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

			prefab.Level = Run.Level;
			loaded[handle.NameWithoutExtension] = prefab;
			
			if (Engine.Version.Debug) {
				var path = handle.ParentName;

				// Fixme: broken on my laptop
				if (!paths.Contains(path)) {
					paths.Add(path);

					var watcher = new FileSystemWatcher();

					watcher.Filter = "*.json";
					watcher.Path = path;
					watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
					                                                | NotifyFilters.FileName | NotifyFilters.DirectoryName;

					watcher.Changed += OnChanged;
					watcher.Created += OnChanged;

					watcher.EnableRaisingEvents = true;

					Log.Debug($"Started watching folder {path}");
				}
			}
		}
		
		private static void OnChanged(object sender, FileSystemEventArgs args) {
			Log.Debug($"Reloading {args.FullPath}");
			Load(new FileHandle(args.FullPath));
		}

		public static void Destroy() {
			loaded.Clear();
		}
	}
}