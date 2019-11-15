using BurningKnight.entity.creature.player;
using BurningKnight.save.statistics;
using BurningKnight.state;
using Lens.entity;
using Lens.util.file;

namespace BurningKnight.save {
	public class PlayerSave : EntitySaver {
		public override void Save(Area area, FileWriter writer) {
			SmartSave(area.Tags[Tags.PlayerSave], writer);
		}

		public override string GetPath(string path, bool old = false) {
			return $"{path}player.sv";
		}

		public override void Generate(Area area) {
			area.Add(new LocalPlayer());

			if (Run.Depth > 0) {
				area.Add(new RunStatistics());
				area.Add(new entity.creature.bk.BurningKnight());
			}
		}

		public PlayerSave() : base(SaveType.Player) {
			
		}
	}
}