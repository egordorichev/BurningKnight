using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;

namespace BurningKnight.entity.level.rooms.connection {
	public class ChasmTunnelRoomDef : TunnelRoomDef {
		protected override void Fill(Level Level) {
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.CHASM);

			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.TUNNEL);
		}

		public override int GetMinHeight() {
			return 7;
		}

		public override int GetMinWidth() {
			return 7;
		}
	}
}