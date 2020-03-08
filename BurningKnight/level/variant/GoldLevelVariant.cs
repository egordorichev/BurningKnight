namespace BurningKnight.level.variant {
	public class GoldLevelVariant : LevelVariant {
		public GoldLevelVariant() : base(LevelVariant.Gold) {
			
		}

		public override void ModifyPainter(Painter painter) {
			base.ModifyPainter(painter);
			Painter.AllGold = true;
		}
	}
}