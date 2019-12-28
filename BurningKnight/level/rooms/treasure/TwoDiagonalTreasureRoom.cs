using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.rooms.treasure {
	public class TwoDiagonalTreasureRoom : TreasureRoom {
		public override void Paint(Level level) {
			if (Rnd.Chance()) {
				var t = Rnd.Chance(30) ? Tile.FloorD : Tiles.RandomFloor();

				if (Rnd.Chance()) {
					Painter.Rect(level, new Rect(Left + 2, Top + 2, Right - 2, Bottom - 2), Rnd.Int(0, 2), t);
				} else {
					Painter.Rect(level, new Rect(Left + 2, Top + 2, Right - 2, Bottom - 2), Rnd.Int(0, 2), t);
				}
			}

			var a = Rnd.Chance();

			if (!a && Rnd.Chance()) {
				Painter.Set(level, GetTileCenter(), Tiles.RandomSolid());
			}
			
			if (Rnd.Chance()) {
				PlaceStand(level, new Dot(Left + 3, Top + 3));
				PlaceStand(level, new Dot(Right - 3, Bottom - 3));	
			} else {
				PlaceStand(level, new Dot(Right - 3, Top + 3));
				PlaceStand(level, new Dot(Left + 3, Bottom - 3));	
			}

			if (a) {
				PlaceStand(level, GetTileCenter());	
			}
			
			SetupStands(level);
		}
		
		public override int GetMinWidth() {
			return 9;
		}

		public override int GetMinHeight() {
			return 9;
		}
		
		public override int GetMaxWidth() {
			return 12;
		}

		public override int GetMaxHeight() {
			return 12;
		}

		protected override bool Quad() {
			return Rnd.Chance();
		}
	}
}