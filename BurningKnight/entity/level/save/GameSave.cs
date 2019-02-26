using BurningKnight.entity.creature.player;
using BurningKnight.util;
using Lens.util.file;

namespace BurningKnight.entity.level.save {
	public class GameSave {
		public static bool DefeatedBK;
		public static int KillCount;
		public static float Time;
		public static bool Inventory;
		public static bool PlayedAlpha;
		public static int RunId;

		public static void Save(FileWriter Writer, bool Old) {
			try {
				Writer.WriteByte((byte) (Old ? Dungeon.LastDepth : Dungeon.Depth));
				Writer.WriteByte((byte) (Player.Instance == null ? Player.GetTypeId(Player.ToSet) : Player.GetTypeId(Player.Instance.Type)));
				Writer.WriteBoolean(DefeatedBK);
				Writer.WriteInt32(KillCount);
				Writer.WriteFloat(Time);
				Writer.WriteBoolean(Inventory);
				Writer.WriteBoolean(PlayedAlpha);
				Writer.WriteInt32(RunId);
				Writer.WriteString(Random.GetSeed());
			}
			catch (IOException) {
				E.PrintStackTrace();
			}
		}

		public static Info Peek(int Slot) {
			FileHandle Save = Gdx.Files.External(SaveManager.GetSavePath(SaveManager.Type.GAME, Slot));
			var Info = new Info();

			if (!Save.Exists()) {
				Info.Free = true;

				return Info;
			}

			FileHandle Sv = Gdx.Files.External(SaveManager.GetSavePath(SaveManager.Type.PLAYER, Slot));

			if (!Sv.Exists()) {
				Info.Free = true;

				return Info;
			}

			try {
				var Stream = new FileReader(Save.File().GetAbsolutePath());
				var Version = Stream.ReadByte();
				Info.Depth = Stream.ReadByte();
				Info.Type = Player.Type.Values()[Stream.ReadByte()];
				Stream.Close();
			}
			catch (Exception) {
				E.PrintStackTrace();
				Info.Error = true;
			}

			return Info;
		}

		public static void Load(FileReader Reader) {
			var D = Reader.ReadByte();
			Player.ToSet = Player.Type.Values()[Reader.ReadByte()];
			DefeatedBK = Reader.ReadBoolean();
			KillCount = Reader.ReadInt32();
			Time = Reader.ReadFloat();
			Inventory = Reader.ReadBoolean();
			PlayedAlpha = Reader.ReadBoolean();
			RunId = Reader.ReadInt32();
			Random.SetSeed(Reader.ReadString());
		}

		public static void Generate() {
			BurningKnight.Instance = null;
			Player.Instance = null;
			KillCount = 0;
			Time = 0;
			DefeatedBK = false;
			Inventory = false;
			PlayedAlpha = false;
		}

		public static class Info {
			public byte Depth;
			public bool Error;
			public bool Free;
			public string Second;
			public Player.Type Type;
		}
	}
}