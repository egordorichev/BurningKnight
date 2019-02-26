using BurningKnight.state;
using Lens.entity;
using Lens.util.file;
using Lens.util.math;

namespace BurningKnight.save {
	public class GameSave {

		public static void Save(Area area, FileWriter Writer, bool Old) {
			Writer.WriteByte((byte) (Old ? Run.LastDepth : Run.Depth));
			Writer.WriteInt32(Run.KillCount);
			Writer.WriteFloat(Run.Time);
			Writer.WriteInt32(Run.Id);
			Writer.WriteString(Random.Seed);
		}

		public static void Load(Area area, FileReader Reader) {
			Run.KillCount = Reader.ReadInt32();
			Run.Time = Reader.ReadFloat();
			Run.Id = Reader.ReadInt32();
			
			Random.Seed = Reader.ReadString();
		}

		public static void Generate(Area area) {
			Run.KillCount = 0;
			Run.Time = 0;
		}
	}
}