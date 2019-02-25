using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class TriangleRoom : RegularRoom {
		enum Type {
			TOP_RIGHT,
			TOP_LEFT,
			BOTTOM_RIGHT,
			BOTTOM_LEFT
		}

		private Type Type;

		public TriangleRoom() {
			this.Type = Type.Values()[Random.NewInt(4)];
		}

		public override Void Paint(Level Level) {
			base.Paint(Level);
			byte F = Terrain.WALL;

			if (Random.Chance(30)) {
				F = Terrain.CHASM;
			} else if (Random.Chance(25)) {
				F = Terrain.LAVA;
			} 

			if (this.Type == Type.BOTTOM_LEFT || this.Type == Type.TOP_LEFT) {
				for (int Y = this.Top + (Type == Type.TOP_LEFT ? 2 : 1); Y < this.Bottom - (Type == Type.TOP_LEFT ? 0 : 1); Y++) {
					Painter.DrawLine(Level, new Point(this.Right - 1, Y), new Point(this.Left + 2, this.Type == Type.BOTTOM_LEFT ? this.Top + 1 : this.Bottom - 1), F);
				}
			} else {
				for (int X = this.Left + 1; X < this.Right - 1; X++) {
					Painter.DrawLine(Level, new Point(X, this.Type == Type.BOTTOM_RIGHT ? this.Top + 1 : this.Bottom - 1), new Point(this.Left + 1, this.Type == Type.BOTTOM_RIGHT ? this.Bottom - 2 : this.Top + 2), F);
				}
			}


			if (Random.Chance(50)) {
				byte Ff = F == Terrain.LAVA ? Terrain.DIRT : Terrain.RandomFloor();
				bool Rect = Random.Chance(50);

				if (Rect) {
					Painter.Fill(Level, this, 3, Ff);
				} else {
					Painter.FillEllipse(Level, this, 3, Ff);
				}


				if (Random.Chance(50)) {
					if (Rect) {
						Painter.Fill(Level, this, 5, F);
					} else {
						Painter.FillEllipse(Level, this, 5, F);
					}

				} 
			} 
		}

		public override bool CanConnect(Point P) {
			if ((this.Type == Type.TOP_RIGHT || this.Type == Type.TOP_LEFT) && P.Y >= this.Bottom) {
				return false;
			} 

			if ((this.Type == Type.BOTTOM_RIGHT || this.Type == Type.BOTTOM_LEFT) && P.Y <= this.Top) {
				return false;
			} 

			if ((this.Type == Type.TOP_RIGHT || this.Type == Type.BOTTOM_RIGHT) && P.X == this.Left) {
				return false;
			} 

			if ((this.Type == Type.TOP_LEFT || this.Type == Type.BOTTOM_LEFT) && P.X == this.Right) {
				return false;
			} 

			return base.CanConnect(P);
		}
	}
}
