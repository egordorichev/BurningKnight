using BurningKnight.state;
using Lens.entity;
using Lens.util;
using Lens.util.file;
using Random = Lens.util.math.Random;

namespace BurningKnight.save {
	public class GameSave : Saver {
		public override void Save(Area area, FileWriter writer) {
			Log.Error($"Wrote {Run.Depth}");
			writer.WriteSbyte((sbyte) Run.Depth);
			writer.WriteInt32(Run.KillCount);
			writer.WriteFloat(Run.Time);
			writer.WriteString(Random.Seed);
		}

		public override string GetPath(string path, bool old = false) {
			return $"{path}game.sv";
		}

		public override void Load(Area area, FileReader reader) {
			if (Run.HasRun) {
				return;
			}

			Run.HasRun = true;
			var d = reader.ReadSbyte();
			// Run.SetDepth(d);
			Log.Error($"Read {d}");
			
			Run.KillCount = reader.ReadInt32();
			Run.Time = reader.ReadFloat();
			
			Random.Seed = reader.ReadString();
		}

		public static int PeekDepth(FileReader reader) {
			var d = reader.ReadSbyte();
			// Run.SetDepth(d);
			Log.Error($"Read d {d}");
			return d;
		}
		
		public override void Generate(Area area) {
			Run.KillCount = 0;
			Run.Time = 0;
		}

		public GameSave() : base(SaveType.Game) {
			
		}
	}
}