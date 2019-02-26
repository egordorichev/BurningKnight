using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class CRoom : RegularRoom {
		private Type Type;

		public CRoom() {
			Type = Type.Values()[Random.NewInt(4)];
		}

		public override void Paint(Level Level) {
			base.Paint(Level);
			var Rect = new Rect();
			Rect.Resize(Random.NewInt(GetWidth() / 4, GetWidth() / 3 * 2), Random.NewInt(GetHeight() / 4, GetHeight() / 3 * 2));

			switch (this.Type) {
				case TOP: {
					Rect.SetPos(Random.NewInt(Left + 2, Right - 1 - Rect.GetWidth()), Top + 1);

					break;
				}

				case BOTTOM: {
					Rect.SetPos(Random.NewInt(Left + 2, Right - 1 - Rect.GetWidth()), Bottom - Rect.GetHeight());

					break;
				}

				case LEFT: {
					Rect.SetPos(Left + 1, Random.NewInt(Top + 2, Bottom - 1 - Rect.GetHeight()));

					break;
				}

				case RIGHT: {
					Rect.SetPos(Right - Rect.GetWidth(), Random.NewInt(Top + 2, Bottom - 1 - Rect.GetHeight()));

					break;
				}
			}

			if (Random.Chance(50)) Painter.Fill(Level, this, 3 + Random.NewInt(3), Terrain.CHASM);

			if (Random.Chance(50)) PaintTunnel(Level, Terrain.RandomFloor(), true);

			var Wall = Random.Chance(50);
			Painter.Fill(Level, Rect, Wall ? Terrain.WALL : Terrain.CHASM);

			if (Random.Chance(50)) Painter.Fill(Level, Rect, 1 + Random.NewInt(2), Wall ? Terrain.CHASM : Terrain.WALL);

			if (Random.Chance(10)) PaintTunnel(Level, Terrain.RandomFloor(), true);
		}

		public override bool CanConnect(Point P) {
			if (this.Type == Type.TOP && P.Y == Top) return false;

			if (this.Type == Type.BOTTOM && P.Y == Bottom) return false;

			if (this.Type == Type.LEFT && P.X == Left) return false;

			if (this.Type == Type.RIGHT && P.X == Right) return false;

			return base.CanConnect(P);
		}

		private enum Type {
			TOP,
			RIGHT,
			LEFT,
			BOTTOM
		}
	}
}