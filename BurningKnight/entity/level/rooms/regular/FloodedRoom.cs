using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;

namespace BurningKnight.entity.level.rooms.regular {
	public class FloodedRoomDef : RegularRoomDef {
		public override void Paint(Level Level) {
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.RandomFloor());
			Painter.Fill(Level, this, 1, Terrain.WATER);

			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.REGULAR);
		}
	}
}