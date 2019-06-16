using Lens.entity;
using Lens.util.file;

namespace BurningKnight.save {
	public class StatisticsSaver : Saver {
		public StatisticsSaver() : base(SaveType.Statistics) {
			
		}

		public override string GetPath(string path, bool old = false) {
			return $"{path}statistics.sv";
		}

		public override void Load(Area area, FileReader reader) {
			
		}

		public override void Save(Area area, FileWriter writer) {
			
		}

		public override void Generate(Area area) {
			
		}
	}
}