namespace BurningKnight.level.variant {
	public class ForestLevelVariant : LevelVariant {
		public ForestLevelVariant() : base(LevelVariant.Forest) {
			
		}

		public override void ModifyPainter(Painter painter) {
			base.ModifyPainter(painter);
			painter.Grass = 0.4f;
		}
	}
}