using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.rooms.treasure {
	public class TwoDiagonalTreasureRoom : TreasureRoom {
		public override void Paint(Level level) {
			if (Rnd.Chance()) {
				PlaceStand(level, new Dot(Left + 2, Top + 2) * 16);
				PlaceStand(level, new Dot(Right - 2, Bottom - 2) * 16);	
			} else {
				PlaceStand(level, new Dot(Right - 2, Top + 2) * 16);
				PlaceStand(level, new Dot(Left + 2, Bottom - 2) * 16);	
			}

			if (Rnd.Chance()) {
				PlaceStand(level, GetTileCenter() * 16);	
			} else if (Rnd.Chance()) {
				Painter.Set(level, GetTileCenter(), Tiles.RandomSolid());
			}

			if (Rnd.Chance()) {
				var t = Rnd.Chance(30) ? Tile.FloorD : Tiles.RandomFloor();

				if (Rnd.Chance()) {
					Painter.Rect(level, new Rect(Left + 2, Top + 2, Right - 2, Bottom - 2), Rnd.Int(0, 2), t);
				} else {
					Painter.Rect(level, new Rect(Left + 2, Top + 2, Right - 2, Bottom - 2), Rnd.Int(0, 2), t);
				}
			}
			
			SetupStands(level);
		}
		
		public override int GetMinWidth() {
			return 7;
		}

		public override int GetMinHeight() {
			return 7;
		}
		
		public override int GetMaxWidth() {
			return 10;
		}

		public override int GetMaxHeight() {
			return 10;
		}

		protected override bool Quad() {
			return Rnd.Chance();
		}
	}
}