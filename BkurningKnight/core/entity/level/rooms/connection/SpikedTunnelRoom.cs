using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.painters;

namespace BurningKnight.core.entity.level.rooms.connection {
	public class SpikedTunnelRoom : TunnelRoom {
		protected override Void Fill(Level Level) {
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.RandomFloor());
			Painter.Fill(Level, this, 1, Terrain.LAVA);

			foreach (LDoor Door in this.Connected.Values()) {
				Door.SetType(LDoor.Type.REGULAR);
			}
		}

		public override int GetMinHeight() {
			return 5;
		}

		public override int GetMinWidth() {
			return 5;
		}
	}
}
