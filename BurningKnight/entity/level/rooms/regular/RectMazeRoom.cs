using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class RectMazeRoomDef : RegularRoomDef {
		public override void Paint(Level Level) {
			base.Paint(Level);
			var Max = Math.Min(GetWidth(), GetHeight()) / 2 + 1;

			for (var M = 2; M < Max; M++)
				if (M % 2 == 0 && !Random.Chance(50)) {
					Painter.Fill(Level, this, M, Random.Chance(30) ? Terrain.WALL : Terrain.CHASM);

					if (M < Max - 1)
						for (var I = 0; I < (Random.Chance(30) ? 2 : 1); I++) {
							var F = Terrain.RandomFloor();

							if (Random.Chance(50)) {
								if (Random.Chance(50))
									Painter.Set(Level, new Point(Right - M, Random.NewInt(Top + 1 + M, Bottom - M)), F);
								else
									Painter.Set(Level, new Point(Left + M, Random.NewInt(Top + 1 + M, Bottom - M)), F);
							}
							else {
								if (Random.Chance(50))
									Painter.Set(Level, new Point(Random.NewInt(Left + M + 1, Right - M), Top + M), F);
								else
									Painter.Set(Level, new Point(Random.NewInt(Left + M + 1, Right - M), Bottom - M), F);
							}
						}
				}
				else {
					Painter.Fill(Level, this, M, Terrain.RandomFloor());
				}
		}

		protected override bool Quad() {
			return Random.Chance(30);
		}

		protected override int ValidateWidth(int W) {
			return W % 2 == 0 ? W : W - 1;
		}

		protected override int ValidateHeight(int H) {
			return H % 2 == 0 ? H : H - 1;
		}

		public override int GetMaxHeight() {
			return 20;
		}

		public override int GetMaxWidth() {
			return 20;
		}
	}
}