using BurningKnight.entity.level.painters;

namespace BurningKnight.entity.level.levels.tech {
	public class TechLevel : RegularLevel {
		public TechLevel() {
			Terrain.LoadTextures(3);
			this.Uid = 3;
		}

		public override string GetName() {
			return Locale.Get("secret_laboratory");
		}

		public override string GetMusic() {
			return Dungeon.Depth == 0 ? "Gobbeon" : "Pirate bay";
		}

		protected override Painter GetPainter() {
			return new HallPainter().SetGrass(0f).SetWater(0.4f);
		}

		protected override int GetNumConnectionRooms() {
			return 0;
		}
	}
}