using BurningKnight.core.entity.level.features;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.rooms.connection {
	public class TunnelRoom : ConnectionRoom {
		protected Void Fill(Level Level) {

		}

		public override Void Paint(Level Level) {
			this.Fill(Level);
			byte Fl = this is SpikedTunnelRoom ? Terrain.DIRT : (Random.Chance(25) ? (Random.Chance(33) ? Terrain.CHASM : (Random.Chance(50) ? Terrain.WALL : Terrain.LAVA)) : Terrain.RandomFloor());

			if (this.GetWidth() > 4 && this.GetHeight() > 4 && Random.Chance(50)) {
				this.PaintTunnel(Level, Fl, true);
			} 

			if (Fl == Terrain.LAVA) {
				this.PaintTunnel(Level, Terrain.RandomFloor());
			} 

			this.PaintTunnel(Level, (Fl == Terrain.DIRT || Fl == Terrain.LAVA) ? (Random.Chance(50) ? Terrain.WATER : Terrain.DIRT) : Terrain.RandomFloor());

			foreach (LDoor Door in this.Connected.Values()) {
				Door.SetType(LDoor.Type.TUNNEL);
			}
		}
	}
}
