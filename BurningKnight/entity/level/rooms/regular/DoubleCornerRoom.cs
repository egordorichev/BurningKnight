using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class DoubleCornerRoom : RegularRoom {
		private Type Type;

		public DoubleCornerRoom() {
			this.Type = Type.Values()[Random.NewInt(2)];
		}

		public override void Paint(Level Level) {
			base.Paint(Level);

			if (Random.Chance(50))
				Painter.FillEllipse(Level, this, 2 + Random.NewInt(3), Terrain.RandomFloor());
			else
				Painter.Fill(Level, this, 2 + Random.NewInt(3), Terrain.RandomFloor());


			Rect Rect = null;
			Rect Rect2 = null;

			switch (this.Type) {
				case TOP_LEFT: {
					Rect = new Rect(Left + 1, Bottom - GetHeight() / 2 + 1, Left + GetWidth() / 2, Bottom);
					Rect2 = new Rect(Right - GetWidth() / 2 + 1, Top + 1, Right, Top + GetHeight() / 2);

					break;
				}

				case BOTTOM_RIGHT: {
					Rect = new Rect(Right - GetWidth() / 2 + 1, Bottom - GetHeight() / 2 + 1, Right, Bottom);
					Rect2 = new Rect(Left + 1, Top + 1, Left + GetWidth() / 2, Top + GetHeight() / 2);

					break;
				}
			}

			var Wall = Random.Chance(50);
			Painter.Fill(Level, Rect, Wall ? Terrain.WALL : Terrain.CHASM);
			Painter.Fill(Level, Rect2, Wall ? Terrain.WALL : Terrain.CHASM);

			if (Random.Chance(50)) Painter.Fill(Level, Rect, 1, !Wall ? Terrain.WALL : Terrain.CHASM);

			if (Random.Chance(50)) Painter.Fill(Level, Rect2, 1, !Wall ? Terrain.WALL : Terrain.CHASM);

			var X = Left + GetWidth() / 2;
			var Y = Top + GetHeight() / 2;
			var M = 1;
			Rect = new Rect(X - M, Y - M, X + M + 1, Y + M + 1);
			Painter.Fill(Level, Rect, Terrain.RandomFloor());

			if (Random.Chance(50)) Painter.Fill(Level, Rect, 1, Random.Chance(50) ? Terrain.CHASM : Terrain.WALL);
		}

		public override bool CanConnect(Point P) {
			if (this.Type == Type.TOP_LEFT && P.X <= Left + GetWidth() / 2 && P.Y >= Top + GetHeight() / 2) return false;

			if (this.Type == Type.TOP_LEFT && P.X >= Left + GetWidth() / 2 && P.Y <= Top + GetHeight() / 2) return false;

			if (this.Type == Type.BOTTOM_RIGHT && P.X <= Left + GetWidth() / 2 && P.Y <= Top + GetHeight() / 2) return false;

			if (this.Type == Type.BOTTOM_RIGHT && P.X >= Left + GetWidth() / 2 && P.Y >= Top + GetHeight() / 2) return false;

			return base.CanConnect(P);
		}

		private enum Type {
			TOP_LEFT,
			BOTTOM_RIGHT
		}
	}
}