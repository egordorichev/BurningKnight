using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class AnticrossRoomDef : RegularRoomDef {
		public override void Paint(Level Level) {
			base.Paint(Level);
			var F = Random.Chance(33) ? Terrain.WALL : (Random.Chance(50) ? Terrain.LAVA : Terrain.CHASM);
			var Fl = Terrain.RandomFloor();

			if (F == Terrain.LAVA) Fl = Random.Chance(50) ? Terrain.WATER : Terrain.DIRT;

			var W = Random.NewInt(2, GetWidth() / 2 - 4);
			var H = Random.NewInt(2, GetHeight() / 2 - 4);
			Painter.Fill(Level, new Rect(Left + 2, Top + GetHeight() / 2 - W, Right - 1, Top + GetHeight() / 2 + W), F);
			Painter.Fill(Level, new Rect(Left + GetWidth() / 2 - H, Top + 2, Left + GetWidth() / 2 + 2, Bottom - 1), F);

			if (Random.Chance(50)) {
				var Side = Random.NewInt(4);
				W -= 1;
				H -= 1;
				Painter.Fill(Level, new Rect(Left + 2, Top + GetHeight() / 2 - W, Right - (Side == 1 ? 2 : 3), Top + GetHeight() / 2 + W), Fl);
				Painter.Fill(Level, new Rect(Left + GetWidth() / 2 - H, Top + (Side == 2 ? 2 : 3), Left + GetWidth() / 2 + H, Bottom - (Side == 3 ? 1 : 2)), Fl);

				if (F != Terrain.LAVA) Fl = Terrain.RandomFloor();

				W -= 1;
				H -= 1;
				Painter.Fill(Level, new Rect(Left + (Side == 0 ? 2 : 3), Top + GetHeight() / 2 - W, Right - (Side == 1 ? 2 : 3), Top + GetHeight() / 2 + W), Fl);
				Painter.Fill(Level, new Rect(Left + GetWidth() / 2 - H, Top + (Side == 2 ? 2 : 3), Left + GetWidth() / 2 + H, Bottom - (Side == 3 ? 1 : 2)), Fl);
			}
		}
	}
}