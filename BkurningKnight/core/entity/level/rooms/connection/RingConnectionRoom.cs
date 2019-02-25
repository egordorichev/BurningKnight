using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.connection {
	public class RingConnectionRoom : TunnelRoom {
		private Rect ConnSpace;

		public override int GetMinWidth() {
			return Math.Max(5, base.GetMinWidth());
		}

		public override int GetMinHeight() {
			return Math.Max(5, base.GetMinHeight());
		}

		public override Void Paint(Level Level) {
			Painter.Fill(Level, this, Terrain.WALL);

			if (Random.Chance(50)) {
				Painter.Fill(Level, this, 1, Terrain.CHASM);
			} 

			byte Fl = (Random.Chance(25) ? (Random.Chance(33) ? Terrain.CHASM : (Random.Chance(50) ? Terrain.WALL : Terrain.LAVA)) : Terrain.RandomFloor());

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

			Rect Ring = GetConnectionSpace();
			byte Floor = Terrain.RandomFloor();
			Painter.Fill(Level, Ring.Left, Ring.Top, 3, 3, Terrain.RandomFloor());
			Painter.Fill(Level, Ring.Left, Ring.Top, 3, 3, Fl == Terrain.LAVA ? (Random.Chance(50) ? Terrain.WATER : Terrain.DIRT) : Floor);
			Painter.Fill(Level, Ring.Left + 1, Ring.Top + 1, 1, 1, Random.Chance(50) ? Terrain.CHASM : Terrain.WALL);
		}

		protected override Rect GetConnectionSpace() {
			if (ConnSpace == null) {
				Point C = GetDoorCenter();
				C.X = (int) MathUtils.Clamp(Left + 2, Right - 2, C.X);
				C.Y = (int) MathUtils.Clamp(Top + 2, Bottom - 2, C.Y);
				ConnSpace = new Rect((int) C.X - 1, (int) C.Y - 1, (int) C.X + 1, (int) C.Y + 1);
			} 

			return ConnSpace;
		}
	}
}
