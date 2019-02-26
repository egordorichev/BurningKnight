using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;

namespace BurningKnight.entity.level.rooms.connection {
	public class SpikedTunnelRoom : TunnelRoom {
		protected override void Fill(Level Level) {
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.RandomFloor());
			Painter.Fill(Level, this, 1, Terrain.LAVA);

			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.REGULAR);
		}

		public override int GetMinHeight() {
			return 5;
		}

		public override int GetMinWidth() {
			return 5;
		}
	}
}