using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class DoubleCornerRoom : RegularRoom {
		enum Type {
			TOP_LEFT,
			BOTTOM_RIGHT
		}

		private Type Type;

		public DoubleCornerRoom() {
			this.Type = Type.Values()[Random.NewInt(2)];
		}

		public override Void Paint(Level Level) {
			base.Paint(Level);

			if (Random.Chance(50)) {
				Painter.FillEllipse(Level, this, 2 + Random.NewInt(3), Terrain.RandomFloor());
			} else {
				Painter.Fill(Level, this, 2 + Random.NewInt(3), Terrain.RandomFloor());
			}


			Rect Rect = null;
			Rect Rect2 = null;

			switch (this.Type) {
				case TOP_LEFT: {
					Rect = new Rect(this.Left + 1, this.Bottom - this.GetHeight() / 2 + 1, this.Left + this.GetWidth() / 2, this.Bottom);
					Rect2 = new Rect(this.Right - this.GetWidth() / 2 + 1, this.Top + 1, this.Right, this.Top + this.GetHeight() / 2);

					break;
				}

				case BOTTOM_RIGHT: {
					Rect = new Rect(this.Right - this.GetWidth() / 2 + 1, this.Bottom - this.GetHeight() / 2 + 1, this.Right, this.Bottom);
					Rect2 = new Rect(this.Left + 1, this.Top + 1, this.Left + this.GetWidth() / 2, this.Top + this.GetHeight() / 2);

					break;
				}
			}

			bool Wall = Random.Chance(50);
			Painter.Fill(Level, Rect, Wall ? Terrain.WALL : Terrain.CHASM);
			Painter.Fill(Level, Rect2, Wall ? Terrain.WALL : Terrain.CHASM);

			if (Random.Chance(50)) {
				Painter.Fill(Level, Rect, 1, !Wall ? Terrain.WALL : Terrain.CHASM);
			} 

			if (Random.Chance(50)) {
				Painter.Fill(Level, Rect2, 1, !Wall ? Terrain.WALL : Terrain.CHASM);
			} 

			int X = this.Left + this.GetWidth() / 2;
			int Y = this.Top + this.GetHeight() / 2;
			int M = 1;
			Rect = new Rect(X - M, Y - M, X + M + 1, Y + M + 1);
			Painter.Fill(Level, Rect, Terrain.RandomFloor());

			if (Random.Chance(50)) {
				Painter.Fill(Level, Rect, 1, Random.Chance(50) ? Terrain.CHASM : Terrain.WALL);
			} 
		}

		public override bool CanConnect(Point P) {
			if ((this.Type == Type.TOP_LEFT) && (P.X <= this.Left + GetWidth() / 2 && P.Y >= this.Top + this.GetHeight() / 2)) {
				return false;
			} 

			if ((this.Type == Type.TOP_LEFT) && (P.X >= this.Left + GetWidth() / 2 && P.Y <= this.Top + GetHeight() / 2)) {
				return false;
			} 

			if ((this.Type == Type.BOTTOM_RIGHT) && (P.X <= this.Left + GetWidth() / 2 && P.Y <= this.Top + this.GetHeight() / 2)) {
				return false;
			} 

			if ((this.Type == Type.BOTTOM_RIGHT) && (P.X >= this.Left + GetWidth() / 2 && P.Y >= this.Top + GetHeight() / 2)) {
				return false;
			} 

			return base.CanConnect(P);
		}
	}
}
