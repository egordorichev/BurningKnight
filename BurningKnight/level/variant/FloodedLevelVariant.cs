using Lens.util.math;

namespace BurningKnight.level.variant {
	public class FloodedLevelVariant : LevelVariant {
		public FloodedLevelVariant() : base(LevelVariant.Flooded) {
			
		}

		public override void ModifyPainter(Painter painter) {
			base.ModifyPainter(painter);
			painter.Water = 0.8f;
		}
		
		public override void PostInit(Level level) {
			base.PostInit(level);

			if (Rnd.Chance()) {
				level.Rains = true;
			}
		}
	}
}