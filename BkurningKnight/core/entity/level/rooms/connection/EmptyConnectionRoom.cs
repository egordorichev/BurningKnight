using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.rooms.connection {
	public class EmptyConnectionRoom : TunnelRoom {
		public override Void Paint(Level Level) {
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.RandomFloor());
			PaintTunnel(Level, Random.Chance(50) ? Terrain.WALL : Terrain.CHASM, true);
			PaintTunnel(Level, Terrain.RandomFloor());

			foreach (LDoor Door in Connected.Values()) {
				Door.SetType(LDoor.Type.TUNNEL);
			}
		}

		public override int GetMinWidth() {
			return 5;
		}

		public override int GetMinHeight() {
			return 5;
		}
	}
}
