using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class GardenRoomDef : RegularRoomDef {
		public override void Paint(Level Level) {
			var F = Terrain.RandomFloor();
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, F);

			if (Random.Chance(50))
				for (var X = Left + 1; X < Right - 1; X++)
					Painter.DrawLine(Level, new Point(X, Top + 1), new Point(X, Bottom - 1), X % 2 == 0 ? Terrain.DIRT : F);
			else
				for (var Y = Top + 1; Y < Bottom - 1; Y++)
					Painter.DrawLine(Level, new Point(Left + 1, Y), new Point(Right - 1, Y), Y % 2 == 0 ? Terrain.DIRT : F);


			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.REGULAR);
		}
	}
}