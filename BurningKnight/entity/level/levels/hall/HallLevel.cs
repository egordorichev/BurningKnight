using BurningKnight.entity.level.painters;

namespace BurningKnight.entity.level.levels.hall {
	public class HallLevel : RegularLevel {
		public HallLevel() {
			Terrain.LoadTextures(0);
			this.Uid = 0;
		}

		public override bool Same(Level Level) {
			return base.Same(Level) || Level is HallBossLevel;
		}

		public override string GetName() {
			return Locale.Get("old_castle");
		}

		public override string GetMusic() {
			return Dungeon.Depth == -2 ? "Shopkeeper" : (Dungeon.Depth == 0 ? "Gobbeon" : (Dungeon.Depth < 0 ? "Outsider" : "Born to do rogueries"));
		}

		protected override Painter GetPainter() {
			return new HallPainter().SetGrass(0.35f).SetWater(0.35f).SetDirt(0.35f).SetCobweb(0.001f);
		}
	}
}