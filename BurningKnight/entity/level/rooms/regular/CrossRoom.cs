using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class CrossRoomDef : RegularRoomDef {
		public override void Paint(Level Level) {
			base.Paint(Level);
			var Wall = Random.Chance(50);
			Painter.Fill(Level, this, 1, Wall ? Terrain.WALL : Terrain.CHASM);

			if (Wall && Random.Chance(50)) {
				if (Random.Chance(50))
					Painter.Fill(Level, this, 2, Terrain.CHASM);
				else
					Painter.FillEllipse(Level, this, 2, Terrain.CHASM);
			}

			var F = Terrain.RandomFloor();
			Painter.Fill(Level, new Rect(Left + 1, Top + GetHeight() / 2 - 1, Right, Top + GetHeight() / 2 + 1), F);
			Painter.Fill(Level, new Rect(Left + GetWidth() / 2 - 1, Top + 1, Left + GetWidth() / 2 + 1, Bottom), F);

			if (!Wall && Random.Chance(50)) {
				if (Random.Chance(50))
					Painter.Fill(Level, this, 2, F);
				else
					Painter.FillEllipse(Level, this, 2, F);
			}
		}

		public override bool CanConnect(Point P) {
			if (!(P.X == Left + GetWidth() / 2 && P.Y == Top) && !(P.X == Left + GetWidth() / 2 && P.Y == Bottom) && !(P.X == Left && P.Y == Top + GetHeight() / 2) && !(P.X == Right && P.Y == Top + GetHeight() / 2) &&
			    !(P.X == Left + GetWidth() / 2 - 1 && P.Y == Top) && !(P.X == Left + GetWidth() / 2 - 1 && P.Y == Bottom) && !(P.X == Left && P.Y == Top + GetHeight() / 2 - 1) && !(P.X == Right && P.Y == Top + GetHeight() / 2 - 1)) return false;

			return base.CanConnect(P);
		}

		public override int GetMinWidth() {
			return 9;
		}

		public override int GetMinHeight() {
			return 9;
		}
	}
}