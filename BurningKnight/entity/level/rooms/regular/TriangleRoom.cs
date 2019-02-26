using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class TriangleRoom : RegularRoom {
		private Type Type;

		public TriangleRoom() {
			this.Type = Type.Values()[Random.NewInt(4)];
		}

		public override void Paint(Level Level) {
			base.Paint(Level);
			var F = Terrain.WALL;

			if (Random.Chance(30))
				F = Terrain.CHASM;
			else if (Random.Chance(25)) F = Terrain.LAVA;

			if (this.Type == Type.BOTTOM_LEFT || this.Type == Type.TOP_LEFT)
				for (var Y = Top + (Type == Type.TOP_LEFT ? 2 : 1); Y < Bottom - (Type == Type.TOP_LEFT ? 0 : 1); Y++)
					Painter.DrawLine(Level, new Point(Right - 1, Y), new Point(Left + 2, this.Type == Type.BOTTOM_LEFT ? Top + 1 : Bottom - 1), F);
			else
				for (var X = Left + 1; X < Right - 1; X++)
					Painter.DrawLine(Level, new Point(X, this.Type == Type.BOTTOM_RIGHT ? Top + 1 : Bottom - 1), new Point(Left + 1, this.Type == Type.BOTTOM_RIGHT ? Bottom - 2 : Top + 2), F);


			if (Random.Chance(50)) {
				var Ff = F == Terrain.LAVA ? Terrain.DIRT : Terrain.RandomFloor();
				var Rect = Random.Chance(50);

				if (Rect)
					Painter.Fill(Level, this, 3, Ff);
				else
					Painter.FillEllipse(Level, this, 3, Ff);


				if (Random.Chance(50)) {
					if (Rect)
						Painter.Fill(Level, this, 5, F);
					else
						Painter.FillEllipse(Level, this, 5, F);
				}
			}
		}

		public override bool CanConnect(Point P) {
			if ((this.Type == Type.TOP_RIGHT || this.Type == Type.TOP_LEFT) && P.Y >= Bottom) return false;

			if ((this.Type == Type.BOTTOM_RIGHT || this.Type == Type.BOTTOM_LEFT) && P.Y <= Top) return false;

			if ((this.Type == Type.TOP_RIGHT || this.Type == Type.BOTTOM_RIGHT) && P.X == Left) return false;

			if ((this.Type == Type.TOP_LEFT || this.Type == Type.BOTTOM_LEFT) && P.X == Right) return false;

			return base.CanConnect(P);
		}

		private enum Type {
			TOP_RIGHT,
			TOP_LEFT,
			BOTTOM_RIGHT,
			BOTTOM_LEFT
		}
	}
}