using System;
using BurningKnight.state;
using Lens.entity;
using Lens.util.file;
using Random = Lens.util.math.Random;

namespace BurningKnight.save {
	public class GameSave {

		public static void Save(Area area, FileWriter Writer) {
			Writer.WriteByte((byte) Run.Depth);
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
			Run.Id = Math.Max(Run.Id, 0);
		}
	}
}