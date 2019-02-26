using System.Threading;
using BurningKnight.state;
using BurningKnight.util;
using Lens.entity;
using Lens.util.file;

namespace BurningKnight.save {
	public class SaveManager {
		public const string SaveDir = "burning_knight/";
		public static int Slot = 0;

		public enum Type {
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
				Save(area, Type.Level, false);
				Save(area, Type.Player, false);
			});
			
			thread.Start();
		}
		
		public static void SaveGames(Area area) {
			var thread = new Thread(() => {
				Save(area, Type.Game, false);
				Save(area, Type.Global, false);
			});
			
			thread.Start();
		}

		public static string GetSavePath(Type Type, bool Old = false) {
			switch (Type) {
				case Type.Level: {
					return $"{((Old ? Run.LastDepth : Run.Depth) <= -1 ? SaveDir : GetDir())}level{(Old ? Run.LastDepth : Run.Depth)}.sv";
				}
				
				case Type.Player: {
					return GetDir() + "player.sv";
				}
				
				case Type.Global: {
					return SaveDir + "global.sv";
				}
				
				default: {
					return GetDir() + "game.sv";
				}
			}
		}

		public static string GetSavePath(Type Type, int Slot) {
			switch (Type) {
				case Type.Level: {
					return $"{(Run.Depth <= -1 ? SaveDir : GetDir(Slot))}level{Run.Depth}.sv";
				}
				
				case Type.Player: {
					return GetDir(Slot) + "player.sv";
				}
				
				case Type.Global: {
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

		public static void Save(Area area, Type Type, bool Old) {
			Log.Info("Saving " + Type + " " + (Old ? Run.LastDepth : Run.Depth));

			var Stream = new FileWriter(GetFileHandle(GetSavePath(Type, Old)).FullPath);

			switch (Type) {
				case Type.Level: {
					LevelSave.Save(area, Stream);
					break;
				}
				
				case Type.Player: {
					PlayerSave.Save(area, Stream);
					break;
				}
				
				case Type.Game: {
					GameSave.Save(area, Stream, Old);
					break;
				}
				
				case Type.Global: {
					GlobalSave.Save(area, Stream);
					break;
				}
			}

			Stream.Close();
		}

		public static bool Load(Area area, Type Type, bool AutoGen = true) {
			var save = GetFileHandle(GetSavePath(Type));

			if (!save.Exists()) {
				if (AutoGen) {
					Generate(area, Type);
					Save(area, Type, false);

					if (Type == Type.Level) {
						Save(area, Type.Game, false);
					}
				} else {
					return false;
				}
			} else {
				Log.Info($"Loading {Type} {Run.Depth}");
				var Stream = new FileReader(save.FullPath);

				switch (Type) {
					case Type.Level: {
						LevelSave.Load(area, Stream);
						break;
					}
					
					case Type.Player: {
						PlayerSave.Load(area, Stream);
						break;
					}
					
					case Type.Game: {
						GameSave.Load(area, Stream);
						break;
					}
					
					case Type.Global: {
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
			FileHandle Handle = GetFileHandle(GetSavePath(Type.Player, Slot));

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
				var Handle = GetFileHandle(GetSavePath(Type.Level));

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

		public static void Generate(Area area, Type Type) {
			Log.Info($"Generating {Type} {Run.Depth}");

			Run.LastDepth = Run.Depth;

			switch (Type) {
				case Type.Level: {
					LevelSave.Generate(area);
					break;
				}
				
				case Type.Player: {
					PlayerSave.Generate(area);
					break;
				}
				
				case Type.Game: {
					GameSave.Generate(area);
					break;
				}
				
				case Type.Global: {
					GlobalSave.Generate(area);
					break;
				}
			}
		}
	}
}