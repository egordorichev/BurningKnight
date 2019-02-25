using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.painters;

namespace BurningKnight.core.entity.level.rooms.connection {
	public class ChasmTunnelRoom : TunnelRoom {
		protected override Void Fill(Level Level) {
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.CHASM);

			foreach (LDoor Door in this.Connected.Values()) {
				Door.SetType(LDoor.Type.TUNNEL);
			}
		}

		public override int GetMinHeight() {
			return 7;
		}

		public override int GetMinWidth() {
			return 7;
		}
	}
}
