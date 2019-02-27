using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;
using BurningKnight.util;

namespace BurningKnight.entity.level.rooms.regular {
	public class CircleRoomDef : RegularRoomDef {
		public override void Paint(Level Level) {
			var F = Terrain.RandomFloor();
			Painter.Fill(Level, this, Terrain.WALL);

			if (Random.Chance(50)) Painter.Fill(Level, this, 1, Terrain.CHASM);

			Painter.FillEllipse(Level, this, 1, F);
			Painter.FillEllipse(Level, this, 2, Terrain.RandomFloor());

			if (Random.Chance(50)) {
			}

			PaintTunnel(Level, Terrain.RandomFloor(), true);

			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.REGULAR);
		}
	}
}