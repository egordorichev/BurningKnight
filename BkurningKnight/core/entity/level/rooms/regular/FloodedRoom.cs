using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.painters;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class FloodedRoom : RegularRoom {
		public override Void Paint(Level Level) {
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.RandomFloor());
			Painter.Fill(Level, this, 1, Terrain.WATER);

			foreach (LDoor Door in this.Connected.Values()) {
				Door.SetType(LDoor.Type.REGULAR);
			}
		}
	}
}
