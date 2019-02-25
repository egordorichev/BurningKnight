using BurningKnight.core.assets;
using BurningKnight.core.entity.level.painters;

namespace BurningKnight.core.entity.level.levels.forest {
	public class ForestLevel : RegularLevel {
		public ForestLevel() {
			Terrain.LoadTextures(5);
			this.Uid = 5;
		}

		public override string GetName() {
			return Locale.Get("forest_ruins");
		}

		public override string GetMusic() {
			return Dungeon.Depth == 0 ? "Gobbeon" : "Botanical Expedition";
		}

		protected override Painter GetPainter() {
			return new HallPainter().SetGrass(0.45f).SetWater(0.45f);
		}
	}
}
