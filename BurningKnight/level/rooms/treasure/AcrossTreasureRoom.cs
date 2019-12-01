using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.rooms.treasure {
	public class AcrossTreasureRoom : TreasureRoom {
		public override void Paint(Level level) {
			var w = (GetWidth() - 1) / 2;
			var h = (GetHeight() - 1) / 2;

			var vr = Rnd.Chance();
			var hr = !vr || Rnd.Chance();

			if (vr) {
				PlaceStand(level, new Dot(Left + w, Top + 2) * 16);
				PlaceStand(level, new Dot(Left + w, Bottom - 2) * 16);
			}

			if (hr) {
				PlaceStand(level, new Dot(Right - 2, Top + h) * 16);
				PlaceStand(level, new Dot(Left + 2, Top + h) * 16);
			}

			if (Rnd.Chance()) {
				PlaceStand(level, new Dot(Left + w, Top + h) * 16);	
			} else if (Rnd.Chance()) {
				Painter.Set(level, GetTileCenter(), Tiles.RandomSolid());
			}

			SetupStands(level);
		}
		
		protected override int ValidateWidth(int W) {
			return W % 2 == 0 ? W : W - 1;
		}
		
		protected override int ValidateHeight(int H) {
			return H % 2 == 0 ? H : H - 1;
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