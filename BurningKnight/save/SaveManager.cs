using System;
using System.IO;
using System.Threading;
using BurningKnight.state;
using Lens;
using Lens.entity;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.save {
	public class SaveManager {
		public const string SaveDir = "burning_knight/";
		public const int MagicNumber = 894923782;
		public const short Version = 0;

		public static byte CurrentSlot = 0;
		public static string SlotDir = $"{SaveDir}slot-{CurrentSlot}/";

		public static Saver[] Savers;
		
		static SaveManager() {
			Log.Info($"Save directory is '{SaveDir}'");

			Savers = new Saver[5];
			Savers[(int) SaveType.Global] = new GlobalSave();
			Savers[(int) SaveType.Game] = new GameSave();
			Savers[(int) SaveType.Level] = new LevelSave();
			Savers[(int) SaveType.Player] = new PlayerSave();
			Savers[(int) SaveType.Secret] = new SecretSave();
			
			var saveDirectory = new FileHandle(SaveDir);

			if (!saveDirectory.Exists()) {
				SecretSave.HadSaveBefore = false;
			}
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

		public static void Save(Area area, SaveType saveType, bool old = false, string path = null) {
			var file = new FileInfo(GetSavePath(saveType, old, path));

			if (saveType != SaveType.Secret || Engine.Version.Dev) {
				Log.Info($"Saving {saveType} {(old ? Run.LastDepth : Run.Depth)} to {file.FullName}");
			}

			file.Directory?.Create();
			
			var stream = new FileWriter(file.FullName);
			stream.WriteInt32(MagicNumber);
			stream.WriteInt16(Version);
			ForType(saveType).Save(area, stream);
			stream.Close();

			if (saveType != SaveType.Secret) {
				SecretSave.HadSaveBefore = true;
			}
		}

		public static void ThreadSave(Action callback, Area area, SaveType saveType, bool old = false, string path = null) {
			new Thread(() => {
				Save(area, saveType, old, path);
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
				if (saveType != SaveType.Secret || Engine.Version.Dev) {
					Log.Info($"Loading {saveType} {Run.Depth}{(path == null ? $" from {save.FullPath}" : $" from {path}")}");
				}

				var stream = new FileReader(save.FullPath);

				if (stream.ReadInt32() != MagicNumber) {
					Log.Error("Invalid magic number!");
					Generate(area, saveType);
					return;
				}

				var version = stream.ReadInt16();

				if (version > Version) {
					Log.Error($"Unknown version {version}, generating new");
					Generate(area, saveType);
				} else if (version < Version) {
					// do something on it
				}
				
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
			if (saveType != SaveType.Secret || Engine.Version.Dev) {
				Log.Info($"Generating {saveType} {Run.Depth}");
			}
			
			ForType(saveType).Generate(area);
			Save(area, saveType, false, null);
		}

		public static void Delete(params SaveType[] types) {
			foreach (var type in types) {
				if (type != SaveType.Secret || Engine.Version.Dev) {
					Log.Info($"Deleting {type} save");
				}

				ForType(type).Delete();
			}
		}
	}
}