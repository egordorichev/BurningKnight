using BurningKnight.level.tile;

namespace BurningKnight.level.variant {
	public class SnowLevelVariant  : LevelVariant {
		public SnowLevelVariant() : base(LevelVariant.Snow) {
			
		}

		public override void ModifyPainter(Painter painter) {
			base.ModifyPainter(painter);
			
			painter.Dirt = 0.5f;
			painter.DirtTile = Tile.Snow;

			painter.Modifiers.Add((l, rm, x, y) => {
				if (l.Get(x, y, true) == Tile.Dirt) {
					l.Set(x, y, Tile.Snow);
				}
			});
		}

		public override void PostInit(Level level) {
			base.PostInit(level);
			level.Snows = true;
		}
	}
}