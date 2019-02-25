using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class CRoom : RegularRoom {
		enum Type {
			TOP,
			RIGHT,
			LEFT,
			BOTTOM
		}

		private Type Type;

		public CRoom() {
			Type = Type.Values()[Random.NewInt(4)];
		}

		public override Void Paint(Level Level) {
			base.Paint(Level);
			Rect Rect = new Rect();
			Rect.Resize(Random.NewInt(this.GetWidth() / 4, this.GetWidth() / 3 * 2), Random.NewInt(this.GetHeight() / 4, this.GetHeight() / 3 * 2));

			switch (this.Type) {
				case TOP: {
					Rect.SetPos(Random.NewInt(this.Left + 2, this.Right - 1 - Rect.GetWidth()), this.Top + 1);

					break;
				}

				case BOTTOM: {
					Rect.SetPos(Random.NewInt(this.Left + 2, this.Right - 1 - Rect.GetWidth()), this.Bottom - Rect.GetHeight());

					break;
				}

				case LEFT: {
					Rect.SetPos(this.Left + 1, Random.NewInt(this.Top + 2, this.Bottom - 1 - Rect.GetHeight()));

					break;
				}

				case RIGHT: {
					Rect.SetPos(this.Right - Rect.GetWidth(), Random.NewInt(this.Top + 2, this.Bottom - 1 - Rect.GetHeight()));

					break;
				}
			}

			if (Random.Chance(50)) {
				Painter.Fill(Level, this, 3 + Random.NewInt(3), Terrain.CHASM);
			} 

			if (Random.Chance(50)) {
				PaintTunnel(Level, Terrain.RandomFloor(), true);
			} 

			bool Wall = Random.Chance(50);
			Painter.Fill(Level, Rect, Wall ? Terrain.WALL : Terrain.CHASM);

			if (Random.Chance(50)) {
				Painter.Fill(Level, Rect, 1 + Random.NewInt(2), Wall ? Terrain.CHASM : Terrain.WALL);
			} 

			if (Random.Chance(10)) {
				PaintTunnel(Level, Terrain.RandomFloor(), true);
			} 
		}

		public override bool CanConnect(Point P) {
			if (this.Type == Type.TOP && P.Y == this.Top) {
				return false;
			} 

			if (this.Type == Type.BOTTOM && P.Y == this.Bottom) {
				return false;
			} 

			if (this.Type == Type.LEFT && P.X == this.Left) {
				return false;
			} 

			if (this.Type == Type.RIGHT && P.X == this.Right) {
				return false;
			} 

			return base.CanConnect(P);
		}
	}
}
