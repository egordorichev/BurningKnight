using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class PrisonRoom : RegularRoom {
		public override int GetMinWidth() {
			return 14;
		}

		public override int GetMaxWidth() {
			return 30;
		}

		public override int GetMinHeight() {
			return 14;
		}

		public override int GetMaxHeight() {
			return 24;
		}

		public override Void Paint(Level Level) {
			base.Paint(Level);
			int CellH = (int) Math.Ceil(((float) this.GetHeight()) / 3) + 1;
			byte F = Terrain.RandomFloor();
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, new Rect(this.Left + 1, this.Top + CellH, this.Right, this.Bottom - CellH + 1), F);

			if (Random.Chance(50)) {
				Painter.Fill(Level, new Rect(this.Left + 1, this.Top + CellH, this.Right, this.Bottom - CellH + 1), 1, Terrain.RandomFloor());
			} 

			for (int Y = 0; Y < 2; Y++) {
				for (int X = 1; X < this.GetWidth(); X += 0) {
					int W = Random.NewInt(6, 8);

					if (W + X + 1 + this.Left < this.Right) {
						PaintSub(Level, this.Left + X, Y == 0 ? this.Top : (this.Bottom - CellH + 1), W + 1, CellH, Y == 0);
					} 

					X += W;
				}
			}
		}

		private Void PaintSub(Level Level, int X, int Y, int W, int H, bool Top) {
			byte F = Terrain.RandomFloor();
			Rect Rc = new Rect(X, Y, X + W, Y + H);
			Painter.Fill(Level, Rc, 1, F);
			int Dx = Random.NewInt(X + 2, X + W - 2);
			int Dy = Y;

			if (Top) {
				Dy = Y + H - 1;
			} 

			Painter.Set(Level, Dx, Dy, F);
			int R = Random.NewInt(6);

			if (R == 0) {
				PaintDead(Level, Rc, Top);
			} else if (R == 1) {
				PaintCollumn(Level, Rc, Top);
			} else if (R == 2) {
				PaintLava(Level, Rc, Top);
			} else if (R == 3) {
				PaintCircle(Level, Rc, Top);
			} else {
				PaintNormal(Level, Rc, Top);
			}

		}

		private Void PaintCircle(Level Level, Rect Self, bool Top) {
			byte F = Terrain.RandomFloor();
			Painter.Fill(Level, Self, 1, Terrain.FLOOR_A);
			Painter.Fill(Level, Self, 1, Random.Chance(50) ? Terrain.WALL : (Random.Chance(60) ? (Random.Chance(50) ? Terrain.CHASM : Terrain.LAVA) : Terrain.FLOOR_D));
			Painter.FillEllipse(Level, Self, 1, F);
		}

		private Void PaintLava(Level Level, Rect Self, bool Top) {
			Painter.Fill(Level, Self, 1, Random.Chance(50) ? Terrain.CHASM : Terrain.LAVA);

			if (Random.Chance(50)) {
				byte F = Terrain.DIRT;
				Painter.Fill(Level, Self, 2, F);
				int M = Top ? 1 : -1;
				Painter.Fill(Level, new Rect(Self.Left, Self.Top + M, Self.Right, Self.Bottom + M), 2, F);
			} 
		}

		private Void PaintDead(Level Level, Rect Self, bool Top) {
			Painter.Fill(Level, Self, 1, Random.Chance(50) ? Terrain.CHASM : Terrain.WALL);
		}

		private Void PaintCollumn(Level Level, Rect Self, bool Top) {
			Painter.Fill(Level, Self, 2, Random.Chance(50) ? Terrain.LAVA : Terrain.WALL);
		}

		private Void PaintNormal(Level Level, Rect Self, bool Top) {

		}

		public override bool CanConnect(Point P) {
			int CellH = (int) Math.Ceil(((float) this.GetHeight()) / 3) + 1;

			if (P.Y < this.Top + CellH || P.Y > this.Bottom - CellH - 1) {
				return false;
			} 

			return base.CanConnect(P);
		}
	}
}
