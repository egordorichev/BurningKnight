using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.entrance {
	public class LineCircleRoomDef : EntranceRoomDef {
		public override void Paint(Level Level) {
			var Floor = Terrain.RandomFloor();
			var Fl = Random.Chance(30) ? Terrain.WALL : (Random.Chance(50) ? Terrain.CHASM : Terrain.LAVA);
			Painter.Fill(Level, this, Terrain.WALL);

			if (Fl == Terrain.LAVA) {
				Floor = Random.Chance(40) ? Terrain.WATER : Terrain.DIRT;
				Painter.Fill(Level, this, 1, Terrain.RandomFloor());
			}

			Painter.Fill(Level, this, 1, Floor);
			Painter.FillEllipse(Level, this, 2, Fl);
			Painter.FillEllipse(Level, this, 3, Floor);
			var F = Floor;
			var S = false;

			if (Random.Chance(50)) {
				Painter.Set(Level, new Point(GetWidth() / 2 + Left, Top + 2), F);
				S = true;
			}

			if (Random.Chance(50)) {
				Painter.Set(Level, new Point(GetWidth() / 2 + Left, Bottom - 2), F);
				S = true;
			}

			if (Random.Chance(50)) {
				Painter.Set(Level, new Point(Left + 2, GetHeight() / 2 + Top), F);
				S = true;
			}

			if (Random.Chance(50) || !S) Painter.Set(Level, new Point(Right - 2, GetHeight() / 2 + Top), F);

			Place(Level, GetCenter());

			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.REGULAR);
		}

		public override int GetMinWidth() {
			return 8;
		}

		public override int GetMinHeight() {
			return 8;
		}
	}
}