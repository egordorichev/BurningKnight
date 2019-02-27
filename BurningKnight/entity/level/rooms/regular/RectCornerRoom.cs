using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class RectCornerRoomDef : RegularRoomDef {
		public override void Paint(Level Level) {
			var F = Terrain.RandomFloor();
			Painter.Fill(Level, this, Terrain.WALL);
			var Chasm = Random.Chance(50);

			if (Chasm) Painter.Fill(Level, this, 1, Terrain.CHASM);

			Painter.Fill(Level, this, 2, F);
			Painter.Fill(Level, this, 3, Chasm ? Terrain.CHASM : Terrain.WALL);

			if (Random.Chance(50)) {
				Painter.FillEllipse(Level, new Rect(Left + 1, Top + 1, Left + 4, Top + 4), F);
				Painter.FillEllipse(Level, new Rect(Right - 3, Top + 1, Right, Top + 4), F);
				Painter.FillEllipse(Level, new Rect(Left + 1, Bottom - 3, Left + 4, Bottom), F);
				Painter.FillEllipse(Level, new Rect(Right - 3, Bottom - 3, Right, Bottom), F);
			}
			else {
				Painter.Fill(Level, new Rect(Left + 1, Top + 1, Left + 4, Top + 4), F);
				Painter.Fill(Level, new Rect(Right - 3, Top + 1, Right, Top + 4), F);
				Painter.Fill(Level, new Rect(Left + 1, Bottom - 3, Left + 4, Bottom), F);
				Painter.Fill(Level, new Rect(Right - 3, Bottom - 3, Right, Bottom), F);
			}


			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.REGULAR);
		}

		public override bool CanConnect(Point P) {
			if (!(P.X == Left + 2 && P.Y == Top) && !(P.X == Left && P.Y == Top + 2) && !(P.X == Right - 2 && P.Y == Top) && !(P.X == Right && P.Y == Top + 2) && !(P.X == Left + 2 && P.Y == Bottom) && !(P.X == Left && P.Y == Bottom - 2) &&
			    !(P.X == Right - 2 && P.Y == Bottom) && !(P.X == Right && P.Y == Bottom - 2)) return false;

			return base.CanConnect(P);
		}
	}
}