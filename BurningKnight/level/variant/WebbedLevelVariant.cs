namespace BurningKnight.level.variant {
	public class WebbedLevelVariant : LevelVariant {
		public WebbedLevelVariant() : base(LevelVariant.Webbed) {
			
		}

		public override void ModifyPainter(Painter painter) {
			base.ModifyPainter(painter);
			painter.Cobweb = 0.6f;
		}
	}
}