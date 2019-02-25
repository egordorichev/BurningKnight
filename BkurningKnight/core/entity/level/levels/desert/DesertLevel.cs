using BurningKnight.core.assets;
using BurningKnight.core.entity.level.painters;

namespace BurningKnight.core.entity.level.levels.desert {
	public class DesertLevel : RegularLevel {
		public DesertLevel() {
			Terrain.LoadTextures(1);
			this.Uid = 1;
		}

		public override bool Same(Level Level) {
			return base.Same(Level) || Level is DesertBossLevel;
		}

		public override string GetName() {
			return Locale.Get("desert_ruins");
		}

		public override string GetMusic() {
			return Dungeon.Depth == 0 ? "Gobbeon" : "Believer";
		}

		protected override Painter GetPainter() {
			return new HallPainter().SetGrass(0.45f).SetWater(0f).SetDirt(0.45f);
		}
	}
}
