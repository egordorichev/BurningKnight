using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class CircleRoom : RegularRoom {
		public override Void Paint(Level Level) {
			byte F = Terrain.RandomFloor();
			Painter.Fill(Level, this, Terrain.WALL);

			if (Random.Chance(50)) {
				Painter.Fill(Level, this, 1, Terrain.CHASM);
			} 

			Painter.FillEllipse(Level, this, 1, F);
			Painter.FillEllipse(Level, this, 2, Terrain.RandomFloor());

			if (Random.Chance(50)) {

			} 

			PaintTunnel(Level, Terrain.RandomFloor(), true);

			foreach (LDoor Door in this.Connected.Values()) {
				Door.SetType(LDoor.Type.REGULAR);
			}
		}
	}
}
