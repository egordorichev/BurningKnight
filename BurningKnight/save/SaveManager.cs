using System;
using System.IO;
using System.Threading;
using BurningKnight.save.cloud;
using BurningKnight.state;
using Lens;
using Lens.entity;
using Lens.util;
using Lens.util.file;
using Steamworks;

namespace BurningKnight.save {
	public class SaveManager {
		public static bool EnableCloudSave;
		
		public const string SaveDir = "burning_knight/";
		public const int MagicNumber = 894923782;
		public const short Version = 0;

		public static byte CurrentSlot = 0;
		public static string SlotDir = $"{SaveDir}slot-{CurrentSlot}/";
		public static string BackupDir = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.burning_knight/";

		public static Saver[] Savers;
		
		static SaveManager() {
			Log.Info($"Save directory is '{SaveDir}'");

			Savers = new Saver[6];
			Savers[(int) SaveType.Global] = new GlobalSave();
			Savers[(int) SaveType.Game] = new GameSave();
			Savers[(int) SaveType.Level] = new LevelSave();
			Savers[(int) SaveType.Player] = new PlayerSave();
			Savers[(int) SaveType.Secret] = new SecretSave();
			Savers[(int) SaveType.Statistics] = new StatisticsSaver();
			
			var saveDirectory = new FileHandle(SaveDir);

			if (!saveDirectory.Exists()) {
				SecretSave.HadSaveBefore = false;
			}
		}

		public static Saver ForType(SaveType type) {
			return Savers[(int) type];
		}

		public static string GetSavePath(SaveType saveType, bool old = false, string path = null) {
			return ForType(saveType).GetPath((path ?? (saveType == SaveType.Statistics || saveType == SaveType.Global || 
			                                    (saveType == SaveType.Level && (old ? Run.LastDepth : Run.Depth) < 1) ? 
				                                  SaveDir : SlotDir)), old);
		}

		public static FileHandle GetFileHandle(string path) {			
			return new FileHandle(path);
		}

		private static FileWriter GetWriter(string path) {
			return EnableCloudSave ? new CloudFileWriter(path) : new FileWriter(path);
		}
		
		private static FileReader GetReader(string path) {
			return EnableCloudSave ? new CloudFileReader(path) : new FileReader(path);
		}

		public static void Save(Area area, SaveType saveType, bool old = false, string path = null) {
			var p = GetSavePath(saveType, old, path);
			var file = new FileInfo(p);

			if (saveType != SaveType.Secret || Engine.Version.Dev) {
				Log.Info($"Saving {saveType} {(old ? Run.LastDepth : Run.Depth)} to {file.FullName}");
			}

			file.Directory?.Create();
			
			var stream = GetWriter(p);

			stream.WriteInt32(MagicNumber);
			stream.WriteInt16(Version);
			stream.WriteByte((byte) saveType);

			ForType(saveType).Save(area, stream);
			stream.Close();

			if (saveType != SaveType.Secret) {
				SecretSave.HadSaveBefore = true;
			}
		}

		public static void ThreadSave(Action callback, Area area, SaveType saveType, bool old = false, string path = null) {
			// new Thread(() => {
				Save(area, saveType, old, path);
				callback?.Invoke();
			// }) {
			// 	Priority = ThreadPriority.Lowest
			// }.Start();
		}

		public static bool ExistsAndValid(SaveType saveType, Action<FileReader> action = null, string path = null) {
			var save = GetFileHandle(GetSavePath(saveType, false, path));

			if (!save.Exists()) {
				return false;
			}
			
			var stream = GetReader(save.FullPath);

			if (stream.ReadInt32() != MagicNumber) {
				return false;
			}

			if (stream.ReadInt16() > Version) {
				return false;
			}

			if (stream.ReadByte() != (byte) saveType) {
				return false;
			}
			
			action?.Invoke(stream);
			return true;
		}

		public static void Load(Area area, SaveType saveType, string path = null) {
			var save = GetFileHandle(GetSavePath(saveType, false, path));

			if (!save.Exists()) {
				Generate(area, saveType);
			} else {
				if (saveType != SaveType.Secret || Engine.Version.Dev) {
					Log.Info($"Loading {saveType} {Run.Depth}{(path == null ? $" from {save.FullPath}" : $" from {path}")}");
				}

				var stream = GetReader(save.FullPath);

				if (stream.ReadInt32() != MagicNumber) {
					Log.Error("Invalid magic number!");
					Generate(area, saveType);
					return;
				}

				var version = stream.ReadInt16();

				if (version > Version) {
					Log.Error($"Unknown version {version}, generating new");
					Generate(area, saveType);
					return;
				} else if (version < Version) {
					// do something on it
				}

				if (stream.ReadByte() != (byte) saveType) {
					Log.Error("Save file did not match it's loader type!");
					Generate(area, saveType);
					return;
				}
				
				ForType(saveType).Load(area, stream);
			}
		}

		public static void Generate(Area area, SaveType saveType) {
			if (saveType != SaveType.Secret || Engine.Version.Dev) {
				Log.Info($"Generating {saveType} {Run.Depth}");
			}
			
			ForType(saveType).Generate(area);

			if (Run.Depth > 0) {
				Save(area, saveType);
			}
		}

		public static void Delete(params SaveType[] types) {
			foreach (var type in types) {
				if (type != SaveType.Secret || Engine.Version.Dev) {
					Log.Info($"Deleting {type} save");
				}

				ForType(type).Delete();
			}
		}

		public static void Backup() {
			var backup = new FileHandle(BackupDir);

			if (!backup.Exists()) {
				try {
					backup.MakeDirectory();
				} catch (Exception e) {
					
				}
			}
			
			var save = new FileHandle(SaveDir);

			if (!save.Exists()) {
				return;
			}
			
			Log.Info("Backing up the saves");

			foreach (var folder in backup.ListDirectoryHandles()) {
				try {
					folder.Delete();
				} catch (Exception e) {
					
				}
			}

			foreach (var file in backup.ListFileHandles()) {
				try {
					file.Delete();
				} catch (Exception e) {
					
				}
			}
			
			foreach (var folder in save.ListDirectoryHandles()) {
				try {
					Directory.CreateDirectory($"{backup.FullPath}{folder.Name}");
				} catch (Exception e) {
					
				}
			
				foreach (var file in folder.ListFileHandles()) {
					try {
						File.Copy(file.FullPath, $"{backup.FullPath}{folder.Name}/{file.Name}", true);
					} catch (Exception e) {
					
					}
				}
			}
			
			foreach (var file in save.ListFileHandles()) {
				try {
					File.Copy(file.FullPath, $"{backup.FullPath}{file.Name}", true);
				} catch (Exception e) {
					
				}
			}
		}
	}
}