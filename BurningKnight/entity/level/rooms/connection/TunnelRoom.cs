using BurningKnight.entity.level.features;
using BurningKnight.util;

namespace BurningKnight.entity.level.rooms.connection {
	public class TunnelRoomDef : ConnectionRoomDef {
		protected void Fill(Level Level) {
		}

		public override void Paint(Level Level) {
			Fill(Level);
			var Fl = this is SpikedTunnelRoomDef ? Terrain.DIRT : (Random.Chance(25) ? (Random.Chance(33) ? Terrain.CHASM : (Random.Chance(50) ? Terrain.WALL : Terrain.LAVA)) : Terrain.RandomFloor());

			if (GetWidth() > 4 && GetHeight() > 4 && Random.Chance(50)) PaintTunnel(Level, Fl, true);

			if (Fl == Terrain.LAVA) PaintTunnel(Level, Terrain.RandomFloor());

			PaintTunnel(Level, Fl == Terrain.DIRT || Fl == Terrain.LAVA ? (Random.Chance(50) ? Terrain.WATER : Terrain.DIRT) : Terrain.RandomFloor());

			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.TUNNEL);
		}
	}
}