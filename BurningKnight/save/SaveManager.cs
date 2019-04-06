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
		public static string SlotDir = $"{SaveDir}slot:{CurrentSlot}/";

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
			} else if (!path.EndsWith("/")) {
				path += "/";
			}

			return ForType(saveType).GetPath(Path.Combine(saveType == SaveType.Global ? SaveDir : SlotDir, path), old);
		}

		public static FileHandle GetFileHandle(string path) {			
			return new FileHandle(path);
		}

		public static void Save(Area area, SaveType saveType, bool old = false, string path = null) {
			var file = new FileInfo(GetSavePath(saveType, old, path));
			Log.Info($"Saving {saveType} {(old ? Run.LastDepth : Run.Depth)} to {file.FullName}");
			file.Directory?.Create();
			
			var stream = new FileWriter(file.FullName);
			ForType(saveType).Save(area, stream);
			stream.Close();
		}

		public static void Load(Area area, SaveType saveType, string path = null) {
			var save = GetFileHandle(GetSavePath(saveType, false, path));

			if (!save.Exists()) {
				Generate(area, saveType);
			} else {
				Log.Info($"Loading {saveType} {Run.Depth}");

				var stream = new FileReader(save.FullPath);
				ForType(saveType).Load(area, stream);
				stream.Close();
			}
		}
		
		public static void Generate(Area area, SaveType saveType) {
			Log.Info($"Generating {saveType} {Run.Depth}");
			ForType(saveType).Generate(area);
			Save(area, saveType);
		}

		public static void Delete(params SaveType[] types) {
			foreach (var type in types) {
				Log.Info($"Deleting {type} save");
				ForType(type).Delete();
			}
		}
	}
}