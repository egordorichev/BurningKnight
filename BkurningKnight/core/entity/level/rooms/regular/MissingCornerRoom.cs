using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class MissingCornerRoom : RegularRoom {
		enum Type {
			TOP_LEFT,
			TOP_RIGHT,
			BOTTOM_LEFT,
			BOTTOM_RIGHT
		}

		private Type Type;

		public MissingCornerRoom() {
			this.Type = Type.Values()[Random.NewInt(4)];
		}

		public override Void Paint(Level Level) {
			base.Paint(Level);

			if (Random.Chance(50)) {
				Painter.FillEllipse(Level, this, 2 + Random.NewInt(3), Terrain.RandomFloor());
			} else {
				Painter.Fill(Level, this, 2 + Random.NewInt(3), Terrain.RandomFloor());
			}


			Rect Rect = null;

			switch (this.Type) {
				case TOP_LEFT: {
					Rect = new Rect(this.Left + 1, this.Top + 1, this.Left + this.GetWidth() / 2, this.Top + this.GetHeight() / 2);

					break;
				}

				case TOP_RIGHT: {
					Rect = new Rect(this.Right - this.GetWidth() / 2 + 1, this.Top + 1, this.Right, this.Top + this.GetHeight() / 2);

					break;
				}

				case BOTTOM_LEFT: {
					Rect = new Rect(this.Left + 1, this.Bottom - this.GetHeight() / 2 + 1, this.Left + this.GetWidth() / 2, this.Bottom);

					break;
				}

				case BOTTOM_RIGHT: {
					Rect = new Rect(this.Right - this.GetWidth() / 2 + 1, this.Bottom - this.GetHeight() / 2 + 1, this.Right, this.Bottom);

					break;
				}
			}

			bool Wall = Random.Chance(50);
			Painter.Fill(Level, Rect, Wall ? Terrain.WALL : Terrain.CHASM);

			if (Random.Chance(50)) {
				Painter.Fill(Level, Rect, 2 + Random.NewInt(3), !Wall ? Terrain.WALL : Terrain.CHASM);
			} 
		}

		public override bool CanConnect(Point P) {
			if ((this.Type == Type.TOP_LEFT || this.Type == Type.TOP_RIGHT) && P.Y >= this.Top + this.GetHeight() / 2) {
				return false;
			} 

			if ((this.Type == Type.BOTTOM_LEFT || this.Type == Type.BOTTOM_RIGHT) && P.Y <= this.Top + GetHeight() / 2) {
				return false;
			} 

			if ((this.Type == Type.BOTTOM_LEFT || this.Type == Type.TOP_LEFT) && P.X <= this.Left + GetWidth() / 2) {
				return false;
			} 

			if ((this.Type == Type.BOTTOM_RIGHT || this.Type == Type.TOP_RIGHT) && P.X >= this.Left + GetWidth() / 2) {
				return false;
			} 

			return base.CanConnect(P);
		}
	}
}
