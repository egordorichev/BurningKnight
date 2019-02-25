using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class GardenRoom : RegularRoom {
		public override Void Paint(Level Level) {
			byte F = Terrain.RandomFloor();
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, F);

			if (Random.Chance(50)) {
				for (int X = this.Left + 1; X < this.Right - 1; X++) {
					Painter.DrawLine(Level, new Point(X, this.Top + 1), new Point(X, this.Bottom - 1), X % 2 == 0 ? Terrain.DIRT : F);
				}
			} else {
				for (int Y = this.Top + 1; Y < this.Bottom - 1; Y++) {
					Painter.DrawLine(Level, new Point(this.Left + 1, Y), new Point(this.Right - 1, Y), Y % 2 == 0 ? Terrain.DIRT : F);
				}
			}


			foreach (LDoor Door in this.Connected.Values()) {
				Door.SetType(LDoor.Type.REGULAR);
			}
		}
	}
}
