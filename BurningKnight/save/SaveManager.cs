using System.Threading;
using BurningKnight.state;
using BurningKnight.util;
using Lens.entity;
using Lens.game;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.save {
	public class SaveManager {
		public const string SaveDir = "burning_knight/";
		public static int Slot = 0;

		public enum SaveType {
			Player,
			Game,
			Level,
			Global
		}

		static SaveManager() {
			Log.Info($"Save directory is {SaveDir}");
		}

		public static string GetDir(int slot = -1) {
			if (slot == -1) {
				slot = Slot;
			}

			return $"{SaveDir}slot-{slot}/";
		}

		public static void SaveGame(Area area) {
			var thread = new Thread(() => {
				Save(area, SaveType.Level, false);
				Save(area, SaveType.Player, false);
			});
			
			thread.Start();
		}
		
		public static void SaveGames(Area area) {
			var thread = new Thread(() => {
				Save(area, SaveType.Game, false);
				Save(area, SaveType.Global, false);
			});
			
			thread.Start();
		}
		
		public static void SaveAll(Area area) {
			var thread = new Thread(() => {
				Save(area, SaveType.Level, false);
				Save(area, SaveType.Player, false);
				Save(area, SaveType.Game, false);
				Save(area, SaveType.Global, false);

				area.Destroy();
			});
			
			thread.Start();
		}

		public static string GetSavePath(SaveType saveType, bool Old = false) {
			switch (saveType) {
				case SaveType.Level: {
					return $"{((Old ? Run.LastDepth : Run.Depth) <= -1 ? SaveDir : GetDir())}level{(Old ? Run.LastDepth : Run.Depth)}.sv";
				}
				
				case SaveType.Player: {
					return GetDir() + "player.sv";
				}
				
				case SaveType.Global: {
					return SaveDir + "global.sv";
				}
				
				default: {
					return GetDir() + "game.sv";
				}
			}
		}

		public static string GetSavePath(SaveType saveType, int Slot) {
			switch (saveType) {
				case SaveType.Level: {
					return $"{(Run.Depth <= -1 ? SaveDir : GetDir(Slot))}level{Run.Depth}.sv";
				}
				
				case SaveType.Player: {
					return GetDir(Slot) + "player.sv";
				}
				
				case SaveType.Global: {
					return SaveDir + "global.sv";
				}

				default: {
					return GetDir(Slot) + "game.sv";
				}
			}
		}

		public static FileHandle GetFileHandle(string Path) {
			return new FileHandle(Path);
		}

		public static void Save(Area area, SaveType saveType, bool Old) {
			Log.Info("Saving " + saveType + " " + (Old ? Run.LastDepth : Run.Depth));

			var file = new System.IO.FileInfo(GetSavePath(saveType, Old));
			file.Directory.Create();
			
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
					GameSave.Save(area, Stream, Old);
					break;
				}
				
				case SaveType.Global: {
					GlobalSave.Save(area, Stream);
					break;
				}
			}

			Stream.Close();
		}

		public static bool Load(Area area, SaveType saveType, bool AutoGen = true) {
			var save = GetFileHandle(GetSavePath(saveType));

			if (!save.Exists()) {
				if (AutoGen) {
					Generate(area, saveType);
					/**Save(area, Type, false);

					if (Type == Type.Level) {
						Save(area, Type.Game, false);
					}*/
				} else {
					return false;
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

			return true;
		}

		public static void DeletePlayer() {
			Log.Info("Deleting player save!");
			FileHandle Handle = GetFileHandle(GetSavePath(SaveType.Player, Slot));

			if (Handle.Exists()) {
				Handle.Delete();
			}
		}

		public static void Delete() {
			Log.Info("Deleting saves!");
			
			var File = GetFileHandle(GetDir());

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
			Run.LastDepth = Run.Depth;

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