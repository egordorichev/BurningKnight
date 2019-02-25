using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.save {
	public class SaveManager {
		enum Type {
			PLAYER,
			GAME,
			LEVEL,
			GLOBAL
		}

		static SaveManager() {
			Log.Info("Save directory is " + SAVE_DIR);
		}

		public const string SAVE_DIR = "burning_knight/";
		public static int Slot = 0;
		public const byte Version = 4;
		public static float Saving;

		public static string GetDir() {
			return GetDir(Slot);
		}

		public static string GetDir(int Slot) {
			return SAVE_DIR + "slot-" + Slot + "/";
		}

		public static string GetSavePath(Type Type) {
			return GetSavePath(Type, false);
		}

		public static Void SaveGame() {
			Thread Thread = new Thread() {
				public override Void Run() {
					base.Run();
					SaveManager.Save(SaveManager.Type.LEVEL, false);
					SaveManager.Save(SaveManager.Type.PLAYER, false);
				}
			};
			Thread.SetPriority(1);
			Thread.Run();
		}

		public static Void SaveGames() {
			Thread Thread = new Thread() {
				public override Void Run() {
					base.Run();
					SaveManager.Save(SaveManager.Type.GAME, false);
					SaveManager.Save(Type.GLOBAL, false);
				}
			};
			Thread.SetPriority(1);
			Thread.Run();
		}

		public static string GetSavePath(Type Type, bool Old) {
			switch (Type) {
				case LEVEL: {
					return ((Old ? Dungeon.LastDepth : Dungeon.Depth) <= -1 ? SAVE_DIR : GetDir()) + "level" + (Old ? Dungeon.LastDepth : Dungeon.Depth) + ".sv";
				}

				case PLAYER: {
					return GetDir() + "player.sv";
				}

				case GAME: 
				default:{
					return GetDir() + "game.sv";
				}

				case GLOBAL: {
					return SAVE_DIR + "global.sv";
				}
			}
		}

		public static string GetSavePath(Type Type, int Slot) {
			switch (Type) {
				case LEVEL: {
					return (Dungeon.Depth <= -1 ? SAVE_DIR : GetDir(Slot)) + "level" + Dungeon.Depth + ".sv";
				}

				case PLAYER: {
					return GetDir(Slot) + "player.sv";
				}

				case GAME: 
				default:{
					return GetDir(Slot) + "game.sv";
				}

				case GLOBAL: {
					return SAVE_DIR + "global.sv";
				}
			}
		}

		public static FileHandle GetFileHandle(string Path) {
			if (Version.Debug) {
				return Gdx.Files.External(Path);
			} else {
				return Gdx.Files.Local(Path);
			}

		}

		public static Void Save(Type Type, bool Old) {
			Saving = 5;
			FileHandle Save = GetFileHandle(GetSavePath(Type, Old));
			Log.Info("Saving " + Type + " " + (Old ? Dungeon.LastDepth : Dungeon.Depth));

			try {
				FileWriter Stream = new FileWriter(Save.File().GetAbsolutePath());
				Stream.WriteByte(Version);

				switch (Type) {
					case LEVEL: {
						LevelSave.Save(Stream);

						break;
					}

					case PLAYER: {
						PlayerSave.Save(Stream);

						break;
					}

					case GAME: {
						GameSave.Save(Stream, Old);

						break;
					}

					case GLOBAL: {
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

		public static Void DeletePlayer() {
			Log.Info("Deleting player save!");
			LevelSave.All.Clear();
			PlayerSave.All.Clear();
			FileHandle Handle = GetFileHandle(GetSavePath(Type.PLAYER, Slot));

			if (Handle.Exists()) {
				Handle.Delete();
			} 
		}

		public static Void Delete() {
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

		public static Void DeleteAll() {
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

		public static Void Generate(Type Type) {
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
