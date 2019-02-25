using BurningKnight.core.entity.creature.player;
using BurningKnight.core.util;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.level.save {
	public class GameSave {
		public static class Info {
			public Player.Type Type;
			public bool Free;
			public bool Error;
			public byte Depth;
			public string Second;
		}

		public static bool DefeatedBK;
		public static int KillCount;
		public static float Time;
		public static bool Inventory;
		public static bool PlayedAlpha;
		public static int RunId;

		public static Void Save(FileWriter Writer, bool Old) {
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
			} catch (IOException) {
				E.PrintStackTrace();
			}
		}

		public static Info Peek(int Slot) {
			FileHandle Save = Gdx.Files.External(SaveManager.GetSavePath(SaveManager.Type.GAME, Slot));
			Info Info = new Info();

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
				FileReader Stream = new FileReader(Save.File().GetAbsolutePath());
				byte Version = Stream.ReadByte();
				Info.Depth = Stream.ReadByte();
				Info.Type = Player.Type.Values()[Stream.ReadByte()];
				Stream.Close();
			} catch (Exception) {
				E.PrintStackTrace();
				Info.Error = true;
			}

			return Info;
		}

		public static Void Load(FileReader Reader) {
			byte D = Reader.ReadByte();
			Player.ToSet = Player.Type.Values()[Reader.ReadByte()];
			DefeatedBK = Reader.ReadBoolean();
			KillCount = Reader.ReadInt32();
			Time = Reader.ReadFloat();
			Inventory = Reader.ReadBoolean();
			PlayedAlpha = Reader.ReadBoolean();
			RunId = Reader.ReadInt32();
			Random.SetSeed(Reader.ReadString());
		}

		public static Void Generate() {
			creature.mob.boss.BurningKnight.Instance = null;
			Player.Instance = null;
			KillCount = 0;
			Time = 0;
			DefeatedBK = false;
			Inventory = false;
			PlayedAlpha = false;
		}
	}
}
