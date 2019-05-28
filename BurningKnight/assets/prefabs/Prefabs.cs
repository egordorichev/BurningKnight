using System.Collections.Generic;
using System.IO;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.util;
using Lens;
using Lens.entity;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.assets.prefabs {
	public static class Prefabs {
		private static Dictionary<string, Prefab> loaded = new Dictionary<string, Prefab>();
		private static List<string> paths = new List<string>();
		private static PrefabSaver saver = new PrefabSaver();
		
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
			
			if (stream.ReadInt32() != SaveManager.MagicNumber) {
				Log.Error("Invalid magic number!");
				return;
			}

			var version = stream.ReadInt16();

			if (version > SaveManager.Version) {
				Log.Error($"Unknown version {version}");
			} else if (version < SaveManager.Version) {
				// do something on it
			}

			if (stream.ReadByte() != (byte) SaveType.Level) {
				return;
			}
			
			saver.Load(new Area { NoInit = true }, stream, false);

			prefab.Level = Run.Level;
			prefab.Datas = ArrayUtils.Clone(saver.Datas);
			
			loaded[handle.NameWithoutExtension] = prefab;
			Run.Level = null;
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