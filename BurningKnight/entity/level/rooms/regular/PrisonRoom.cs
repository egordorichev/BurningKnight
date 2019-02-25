using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
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

		public override void Paint(Level Level) {
			base.Paint(Level);
			var CellH = (int) Math.Ceil((float) GetHeight() / 3) + 1;
			var F = Terrain.RandomFloor();
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, new Rect(Left + 1, Top + CellH, Right, Bottom - CellH + 1), F);

			if (Random.Chance(50)) Painter.Fill(Level, new Rect(Left + 1, Top + CellH, Right, Bottom - CellH + 1), 1, Terrain.RandomFloor());

			for (var Y = 0; Y < 2; Y++)
			for (var X = 1; X < GetWidth(); X += 0) {
				var W = Random.NewInt(6, 8);

				if (W + X + 1 + Left < Right) PaintSub(Level, Left + X, Y == 0 ? Top : Bottom - CellH + 1, W + 1, CellH, Y == 0);

				X += W;
			}
		}

		private void PaintSub(Level Level, int X, int Y, int W, int H, bool Top) {
			var F = Terrain.RandomFloor();
			var Rc = new Rect(X, Y, X + W, Y + H);
			Painter.Fill(Level, Rc, 1, F);
			var Dx = Random.NewInt(X + 2, X + W - 2);
			var Dy = Y;

			if (Top) Dy = Y + H - 1;

			Painter.Set(Level, Dx, Dy, F);
			var R = Random.NewInt(6);

			if (R == 0)
				PaintDead(Level, Rc, Top);
			else if (R == 1)
				PaintCollumn(Level, Rc, Top);
			else if (R == 2)
				PaintLava(Level, Rc, Top);
			else if (R == 3)
				PaintCircle(Level, Rc, Top);
			else
				PaintNormal(Level, Rc, Top);
		}

		private void PaintCircle(Level Level, Rect Self, bool Top) {
			var F = Terrain.RandomFloor();
			Painter.Fill(Level, Self, 1, Terrain.FLOOR_A);
			Painter.Fill(Level, Self, 1, Random.Chance(50) ? Terrain.WALL : (Random.Chance(60) ? (Random.Chance(50) ? Terrain.CHASM : Terrain.LAVA) : Terrain.FLOOR_D));
			Painter.FillEllipse(Level, Self, 1, F);
		}

		private void PaintLava(Level Level, Rect Self, bool Top) {
			Painter.Fill(Level, Self, 1, Random.Chance(50) ? Terrain.CHASM : Terrain.LAVA);

			if (Random.Chance(50)) {
				var F = Terrain.DIRT;
				Painter.Fill(Level, Self, 2, F);
				var M = Top ? 1 : -1;
				Painter.Fill(Level, new Rect(Self.Left, Self.Top + M, Self.Right, Self.Bottom + M), 2, F);
			}
		}

		private void PaintDead(Level Level, Rect Self, bool Top) {
			Painter.Fill(Level, Self, 1, Random.Chance(50) ? Terrain.CHASM : Terrain.WALL);
		}

		private void PaintCollumn(Level Level, Rect Self, bool Top) {
			Painter.Fill(Level, Self, 2, Random.Chance(50) ? Terrain.LAVA : Terrain.WALL);
		}

		private void PaintNormal(Level Level, Rect Self, bool Top) {
		}

		public override bool CanConnect(Point P) {
			var CellH = (int) Math.Ceil((float) GetHeight() / 3) + 1;

			if (P.Y < Top + CellH || P.Y > Bottom - CellH - 1) return false;

			return base.CanConnect(P);
		}
	}
}