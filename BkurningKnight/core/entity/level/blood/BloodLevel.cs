using BurningKnight.core.assets;
using BurningKnight.core.entity.level.painters;

namespace BurningKnight.core.entity.level.blood {
	public class BloodLevel : RegularLevel {
		public BloodLevel() {
			Terrain.LoadTextures(6);
			this.Uid = 6;
		}

		public override string GetName() {
			return Locale.Get("womb_ruins");
		}

		public override string GetMusic() {
			return Dungeon.Depth == 0 ? "Gobbeon" : "Born to do rogueries";
		}

		protected override Painter GetPainter() {
			return new HallPainter().SetGrass(0f).SetWater(0.35f);
		}
	}
}
