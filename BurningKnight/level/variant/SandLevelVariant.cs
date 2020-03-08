using BurningKnight.level.tile;

namespace BurningKnight.level.variant {
	public class SandLevelVariant : LevelVariant {
		public SandLevelVariant() : base(LevelVariant.Sand) {
			
		}

		public override void ModifyPainter(Painter painter) {
			base.ModifyPainter(painter);
			
			painter.Dirt = 0.5f;
			painter.DirtTile = Tile.Sand;
				
			painter.Modifiers.Add((l, rm, x, y) => {
				if (l.Get(x, y, true) == Tile.Dirt) {
					l.Set(x, y, Tile.Sand);
				}
			});
		}
	}
}