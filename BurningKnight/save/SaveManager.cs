using System;
using System.IO;
using System.Threading;
using BurningKnight.state;
using Lens.entity;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.save {
	public class SaveManager {
		public const string SaveDir = "burning_knight/";
		public static byte CurrentSlot = 0;
		public static string SlotDir = $"{SaveDir}slot-{CurrentSlot}/";

		public static Saver[] Savers;
		
		static SaveManager() {
			Log.Info($"Save directory is '{SaveDir}'");

			Savers = new Saver[4];
			Savers[(int) SaveType.Global] = new GlobalSave();
			Savers[(int) SaveType.Game] = new GameSave();
			Savers[(int) SaveType.Level] = new LevelSave();
			Savers[(int) SaveType.Player] = new PlayerSave();
		}

		public static Saver ForType(SaveType type) {
			return Savers[(int) type];
		}

		public static string GetSavePath(SaveType saveType, bool old = false, string path = null) {
			if (path == null) {
				path = "";
			} else if (saveType != SaveType.Level && !path.EndsWith("/")) {
				path += "/";
			}

			return ForType(saveType).GetPath(saveType == SaveType.Level && path != "" ? path : Path.Combine(saveType == SaveType.Global ? SaveDir : SlotDir, path), old);
		}

		public static FileHandle GetFileHandle(string path) {			
			return new FileHandle(path);
		}

		public static void Save(Area area, SaveType saveType, bool old = false, string path = null, bool generated = false, bool autoRemove = true) {
			if (autoRemove) {
				area.AutoRemove();

				if (!generated) {
					area.CleanNew();
				}
			}

			var file = new FileInfo(GetSavePath(saveType, old, path));
			Log.Info($"Saving {saveType} {(old ? Run.LastDepth : Run.Depth)} to {file.FullName}");
			file.Directory?.Create();
			
			var stream = new FileWriter(file.FullName);
			ForType(saveType).Save(area, stream);
			stream.Close();
		}

		public static void ThreadSave(Action callback, Area area, SaveType saveType, bool old = false, string path = null, bool generated = false, bool autoRemove = true) {
			new Thread(() => {
				Save(area, saveType, old, path, generated, autoRemove);
				callback?.Invoke();
			}) {
				Priority = ThreadPriority.Lowest
			}.Start();
		}

		public static void Load(Area area, SaveType saveType, string path = null) {
			var save = GetFileHandle(GetSavePath(saveType, false, path));

			if (!save.Exists()) {
				Generate(area, saveType);
			} else {
				Log.Info($"Loading {saveType} {Run.Depth}{(path == null ? $" from {save.FullPath}" : $" from {path}")}");

				var stream = new FileReader(save.FullPath);
				ForType(saveType).Load(area, stream);
			}
		}

		public static void ThreadLoad(Action callback, Area area, SaveType saveType, string path = null) {
			//new Thread(() => {
				Load(area, saveType, path);
				callback?.Invoke();
			//}) {
			//	Priority = ThreadPriority.Lowest
			//}.Start();
		}
		
		public static void Generate(Area area, SaveType saveType) {
			Log.Info($"Generating {saveType} {Run.Depth}");
			ForType(saveType).Generate(area);
			Save(area, saveType, false, null, true, false);
		}

		public static void Delete(params SaveType[] types) {
			foreach (var type in types) {
				Log.Info($"Deleting {type} save");
				ForType(type).Delete();
			}
		}
	}
}