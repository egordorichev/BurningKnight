using System;
using System.Threading;
using BurningKnight.entity.level.entities;
using BurningKnight.util;
using Lens.util.file;
using IOException = System.IO.IOException;

namespace BurningKnight.entity.level.save {
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

		public static string GetSavePath(Type Type) {
			return GetSavePath(Type, false);
		}

		private static void SaveGameThread() {
			Save(Type.Level, false);
			Save(Type.Player, false);
		}

		public static void SaveGame() {
			var thread = new Thread(SaveGameThread);
			thread.Start();
		}

		private static void SaveGamesThread() {
			Save(Type.Game, false);
			Save(Type.Global, false);
		}

		public static void SaveGames() {
			var thread = new Thread(SaveGamesThread);
			thread.Start();
		}

		public static string GetSavePath(Type Type, bool Old) {
			switch (Type) {
				case Type.Level: {
					return ((Old ? Dungeon.LastDepth : Dungeon.Depth) <= -1 ? SaveDir : GetDir()) + "level" +
					  (Old ? Dungeon.LastDepth : Dungeon.Depth) + ".sv";
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
					return (Dungeon.Depth <= -1 ? SaveDir : GetDir(Slot)) + "level" + Dungeon.Depth + ".sv";
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
			if (Version.Debug) {
				return Gdx.Files.External(Path);
			}
				
			return Gdx.Files.Local(Path);
		}

		public static void Save(Type Type, bool Old) {
			Saving = 5;
			FileHandle Save = GetFileHandle(GetSavePath(Type, Old));
			Log.Info("Saving " + Type + " " + (Old ? Dungeon.LastDepth : Dungeon.Depth));

			try {
				FileWriter Stream = new FileWriter(Save.File().GetAbsolutePath());
				Stream.WriteByte(Version);

				switch (Type) {
					case Type.Level: {
						LevelSave.Save(Stream);
						break;
					}
					
					case Type.Player: {
						PlayerSave.Save(Stream);
						break;
					}
					
					case Type.Game: {
						GameSave.Save(Stream, Old);
						break;
					}
					
					case Type.Global: {
						GlobalSave.Save(Stream);
						break;
					}
				}

				Stream.Close();
			} catch (Exception) {
				Dungeon.ReportException(E);
			}
		}

		public static bool Load(Type Type) {
			return Load(Type, true);
		}

		public static bool Load(Type Type, bool AutoGen) {
			FileHandle Save = GetFileHandle(GetSavePath(Type));

			if (!Save.Exists()) {
				if (AutoGen) {
					File File = Save.File();
					File.GetParentFile().Mkdirs();

					try {
						File.CreateNewFile();
					} catch (IOException) {
						Dungeon.ReportException(E);
					}

					Generate(Type);
					Save(Type, false);

					if (Type == Type.LEVEL) {
						Save(Type.GAME, false);
					}
				} else {
					return false;
				}
			} else {
				Log.Info("Loading " + Type + " " + Dungeon.Depth);
				FileReader Stream = new FileReader(Save.File().GetAbsolutePath());
				byte V = Stream.ReadByte();

				if (V > Version) {
					Log.Error("Unknown save version!");
					Stream.Close();
					Generate(Type);
					return false;
				} else if (V < Version) {
					Log.Info("Older save version!");
				}

				switch (Type) {
					case LEVEL: {
						LevelSave.Load(Stream);
						break;
					}
					case PLAYER: {
						PlayerSave.Load(Stream);
						break;
					}
					case GAME: {
						GameSave.Load(Stream);
						break;
					}
					case GLOBAL: {
						GlobalSave.Load(Stream);
						break;
					}
				}

				Stream.Close();
			}

			return true;
		}

		public static void DeletePlayer() {
			Log.Info("Deleting player save!");
			LevelSave.All.Clear();
			PlayerSave.All.Clear();
			FileHandle Handle = GetFileHandle(GetSavePath(Type.PLAYER, Slot));

			if (Handle.Exists()) {
				Handle.Delete();
			}
		}

		public static void Delete() {
			Log.Info("Deleting saves!");
			LevelSave.All.Clear();
			PlayerSave.All.Clear();
			File File = GetFileHandle(GetDir()).File();

			if (File == null) {
				Log.Error("Failed to delete!");
				return;
			}

			File[] Files = File.ListFiles();

			if (Files == null) {
				File.Delete();
				Log.Error("Failed to detect inner files to delete!");
				return;
			}

			foreach (File F in Files) {
				F.Delete();
			}

			File.Delete();

			if (Dungeon.Depth < 0) {
				FileHandle Handle = GetFileHandle(GetSavePath(Type.LEVEL, false));

				if (Handle.Exists()) {
					Handle.Delete();
				}
			}
		}

		public static void DeleteAll() {
			Log.Info("Deleting all saves!");
			LevelSave.All.Clear();
			PlayerSave.All.Clear();
			FileHandle File = GetFileHandle(SAVE_DIR);

			if (File.Exists()) {
				File.DeleteDirectory();
			}

			GlobalSave.Values.Clear();
			GlobalSave.Generate();
		}

		public static void Generate(Type Type) {
			Dungeon.LoadType = Entrance.LoadType.GO_DOWN;
			Log.Info("Generating " + Type + " " + Dungeon.Depth);
			Dungeon.LastDepth = Dungeon.Depth;

			switch (Type) {
				case LEVEL: {
					LevelSave.Generate();
					break;
				}
				case PLAYER: {
					PlayerSave.Generate();
					break;
				}
				case GAME: {
					GameSave.Generate();
					break;
				}
				case GLOBAL: {
					GlobalSave.Generate();
					break;
				}
			}
		}
	}
}