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
			var a = Rnd.Chance();

			if (!a && Rnd.Chance()) {
				Painter.Set(level, GetTileCenter(), Tiles.RandomSolid());
			}

			if (vr) {
				PlaceStand(level, new Dot(Left + w, Top + 3));
				PlaceStand(level, new Dot(Left + w, Bottom - 3));
			}

			if (hr) {
				PlaceStand(level, new Dot(Right - 3, Top + h));
				PlaceStand(level, new Dot(Left + 3, Top + h));
			}

			if (a) {
				PlaceStand(level, new Dot(Left + w, Top + h));	
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