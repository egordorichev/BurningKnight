using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class RectMazeRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			int Max = Math.Min(this.GetWidth(), this.GetHeight()) / 2 + 1;

			for (int M = 2; M < Max; M++) {
				if (M % 2 == 0 && !Random.Chance(50)) {
					Painter.Fill(Level, this, M, Random.Chance(30) ? Terrain.WALL : Terrain.CHASM);

					if (M < Max - 1) {
						for (int I = 0; I < (Random.Chance(30) ? 2 : 1); I++) {
							byte F = Terrain.RandomFloor();

							if (Random.Chance(50)) {
								if (Random.Chance(50)) {
									Painter.Set(Level, new Point(this.Right - M, Random.NewInt(this.Top + 1 + M, this.Bottom - M)), F);
								} else {
									Painter.Set(Level, new Point(this.Left + M, Random.NewInt(this.Top + 1 + M, this.Bottom - M)), F);
								}

							} else {
								if (Random.Chance(50)) {
									Painter.Set(Level, new Point(Random.NewInt(this.Left + M + 1, this.Right - M), this.Top + M), F);
								} else {
									Painter.Set(Level, new Point(Random.NewInt(this.Left + M + 1, this.Right - M), this.Bottom - M), F);
								}

							}

						}
					} 
				} else {
					Painter.Fill(Level, this, M, Terrain.RandomFloor());
				}

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
