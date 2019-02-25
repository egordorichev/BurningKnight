using BurningKnight.core.assets;
using BurningKnight.core.entity.level.painters;

namespace BurningKnight.core.entity.level.levels.creep {
	public class CreepLevel : RegularLevel {
		public CreepLevel() {
			Terrain.LoadTextures(4);
			this.Uid = 4;
		}

		public override string GetName() {
			return Locale.Get("corrupted_castle");
		}

		public override string GetMusic() {
			return Dungeon.Depth == 0 ? "Gobbeon" : "Believer";
		}

		protected override Painter GetPainter() {
			return new HallPainter().SetGrass(0.45f).SetWater(0.45f);
		}

		protected override int GetNumConnectionRooms() {
			return 0;
		}
	}
}
