using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class MissingCornerRoom : RegularRoom {
		private Type Type;

		public MissingCornerRoom() {
			this.Type = Type.Values()[Random.NewInt(4)];
		}

		public override void Paint(Level Level) {
			base.Paint(Level);

			if (Random.Chance(50))
				Painter.FillEllipse(Level, this, 2 + Random.NewInt(3), Terrain.RandomFloor());
			else
				Painter.Fill(Level, this, 2 + Random.NewInt(3), Terrain.RandomFloor());


			Rect Rect = null;

			switch (this.Type) {
				case TOP_LEFT: {
					Rect = new Rect(Left + 1, Top + 1, Left + GetWidth() / 2, Top + GetHeight() / 2);

					break;
				}

				case TOP_RIGHT: {
					Rect = new Rect(Right - GetWidth() / 2 + 1, Top + 1, Right, Top + GetHeight() / 2);

					break;
				}

				case BOTTOM_LEFT: {
					Rect = new Rect(Left + 1, Bottom - GetHeight() / 2 + 1, Left + GetWidth() / 2, Bottom);

					break;
				}

				case BOTTOM_RIGHT: {
					Rect = new Rect(Right - GetWidth() / 2 + 1, Bottom - GetHeight() / 2 + 1, Right, Bottom);

					break;
				}
			}

			var Wall = Random.Chance(50);
			Painter.Fill(Level, Rect, Wall ? Terrain.WALL : Terrain.CHASM);

			if (Random.Chance(50)) Painter.Fill(Level, Rect, 2 + Random.NewInt(3), !Wall ? Terrain.WALL : Terrain.CHASM);
		}

		public override bool CanConnect(Point P) {
			if ((this.Type == Type.TOP_LEFT || this.Type == Type.TOP_RIGHT) && P.Y >= Top + GetHeight() / 2) return false;

			if ((this.Type == Type.BOTTOM_LEFT || this.Type == Type.BOTTOM_RIGHT) && P.Y <= Top + GetHeight() / 2) return false;

			if ((this.Type == Type.BOTTOM_LEFT || this.Type == Type.TOP_LEFT) && P.X <= Left + GetWidth() / 2) return false;

			if ((this.Type == Type.BOTTOM_RIGHT || this.Type == Type.TOP_RIGHT) && P.X >= Left + GetWidth() / 2) return false;

			return base.CanConnect(P);
		}

		private enum Type {
			TOP_LEFT,
			TOP_RIGHT,
			BOTTOM_LEFT,
			BOTTOM_RIGHT
		}
	}
}