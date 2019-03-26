using System.IO;
using System.Threading;
using BurningKnight.state;
using Lens.entity;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.save {
	public class SaveManager {
		public const string SaveDir = "burning_knight/";

		public enum SaveType {
			Player,
			Game,
			Level,
			Global
		}

		static SaveManager() {
			Log.Info($"Save directory is {SaveDir}");
		}

		public static void SaveGame(Area area) {
			var thread = new Thread(() => {
				Save(area, SaveType.Level);
				Save(area, SaveType.Player);
			});
			
			thread.Start();
		}
		
		public static void SaveGames(Area area) {
			var thread = new Thread(() => {
				Save(area, SaveType.Game);
				Save(area, SaveType.Global);
			});
			
			thread.Start();
		}
		
		public static void SaveAll(Area area) {
			var thread = new Thread(() => {
				Save(area, SaveType.Level);
				Save(area, SaveType.Player);
				Save(area, SaveType.Game);
				Save(area, SaveType.Global);

				area.Destroy();
			});
			
			thread.Start();
		}

		public static string GetSavePath(SaveType saveType) {
			switch (saveType) {
				case SaveType.Level: {
					return $"{SaveDir}level.sv";
				}
				
				case SaveType.Player: {
					return $"{SaveDir}player.sv";
				}
				
				case SaveType.Global: {
					return $"{SaveDir}global.sv";
				}
				
				default: {
					return $"{SaveDir}game.sv";
				}
			}
		}

		public static string PatchSavePath(string path, SaveType saveType) {
			if (!path.EndsWith("/")) {
				path += "/";
			}
			
			path = Path.Combine(SaveDir, path);
			
			switch (saveType) {
				case SaveType.Level: {
					return $"{path}level.sv";
				}
				
				case SaveType.Player: {
					return $"{path}player.sv";
				}
				
				case SaveType.Global: {
					return $"{path}global.sv";
				}
				
				default: {
					return $"{path}game.sv";
				}
			}
		}

		public static FileHandle GetFileHandle(string path) {			
			return new FileHandle(path);
		}

		public static void Save(Area area, SaveType saveType, string path = null) {
			var file = new FileInfo(path == null ? GetSavePath(saveType) : PatchSavePath(path, saveType));
			Log.Info($"Saving {saveType} {Run.Depth} to {file.FullName}");
			
			file.Directory?.Create();
			
			var Stream = new FileWriter(file.FullName);

			switch (saveType) {
				case SaveType.Level: {
					LevelSave.Save(area, Stream);
					break;
				}
				
				case SaveType.Player: {
					PlayerSave.Save(area, Stream);
					break;
				}
				
				case SaveType.Game: {
					GameSave.Save(area, Stream);
					break;
				}
				
				case SaveType.Global: {
					GlobalSave.Save(area, Stream);
					break;
				}
			}

			Stream.Close();
		}

		public static void Load(Area area, SaveType saveType, string path = null, bool AutoGen = true) {
			var save = GetFileHandle(path == null ? GetSavePath(saveType) : PatchSavePath(path, saveType));

			if (!save.Exists()) {
				if (AutoGen) {
					Generate(area, saveType);
				}
			} else {
				Log.Info($"Loading {saveType} {Run.Depth}");
				var Stream = new FileReader(save.FullPath);

				switch (saveType) {
					case SaveType.Level: {
						LevelSave.Load(area, Stream);
						break;
					}
					
					case SaveType.Player: {
						PlayerSave.Load(area, Stream);
						break;
					}
					
					case SaveType.Game: {
						GameSave.Load(area, Stream);
						break;
					}
					
					case SaveType.Global: {
						GlobalSave.Load(area, Stream);
						break;
					}
				}

				Stream.Close();
			}
		}

		public static void DeletePlayer() {
			Log.Info("Deleting player save!");
			var Handle = GetFileHandle(GetSavePath(SaveType.Player));

			if (Handle.Exists()) {
				Handle.Delete();
			}
		}

		public static void Delete() {
			Log.Info("Deleting saves!");
			
			var File = GetFileHandle(SaveDir);

			if (File == null) {
				Log.Error("Failed to delete!");
				return;
			}

			var Files = File.ListFileHandles();

			if (Files == null) {
				File.Delete();
				Log.Error("Failed to detect inner files to delete!");
				return;
			}

			foreach (var F in Files) {
				F.Delete();
			}

			File.Delete();

			if (Run.Depth < 0) {
				var Handle = GetFileHandle(GetSavePath(SaveType.Level));

				if (Handle.Exists()) {
					Handle.Delete();
				}
			}
		}

		public static void DeleteAll(Area area) {
			Log.Info("Deleting all saves!");
			
			var File = GetFileHandle(SaveDir);

			if (File.Exists()) {
				File.Delete();
			}

			GlobalSave.Values.Clear();
			GlobalSave.Generate(area);
		}

		public static void Generate(Area area, SaveType saveType) {
			Log.Info($"Generating {saveType} {Run.Depth}");

			switch (saveType) {
				case SaveType.Level: {
					LevelSave.Generate(area);
					break;
				}
				
				case SaveType.Player: {
					PlayerSave.Generate(area);
					break;
				}
				
				case SaveType.Game: {
					GameSave.Generate(area);
					break;
				}
				
				case SaveType.Global: {
					GlobalSave.Generate(area);
					break;
				}
			}
		}
	}
}