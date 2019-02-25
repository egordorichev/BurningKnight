using BurningKnight.core.assets;
using BurningKnight.core.entity.level.painters;

namespace BurningKnight.core.entity.level.levels {
	public class BossLevel : RegularLevel {
		public BossLevel() {
			Terrain.LoadTextures(0);
			this.Uid = 0;
		}

		public override bool Same(Level Level) {
			return base.Same(Level) || Level is BossLevel;
		}

		public override string GetName() {
			return Locale.Get("old_castle");
		}

		public override string GetMusic() {
			return "Gobbeon";
		}

		protected override Painter GetPainter() {
			return new HallPainter().SetGrass(0.35f).SetWater(0.35f).SetDirt(0.35f).SetCobweb(0.001f);
		}
	}
}
