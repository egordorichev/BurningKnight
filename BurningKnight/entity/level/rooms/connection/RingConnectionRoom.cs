using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.connection {
	public class RingConnectionRoom : TunnelRoom {
		private Rect ConnSpace;

		public override int GetMinWidth() {
			return Math.Max(5, base.GetMinWidth());
		}

		public override int GetMinHeight() {
			return Math.Max(5, base.GetMinHeight());
		}

		public override void Paint(Level Level) {
			Painter.Fill(Level, this, Terrain.WALL);

			if (Random.Chance(50)) Painter.Fill(Level, this, 1, Terrain.CHASM);

			var Fl = Random.Chance(25) ? (Random.Chance(33) ? Terrain.CHASM : (Random.Chance(50) ? Terrain.WALL : Terrain.LAVA)) : Terrain.RandomFloor();

			if (GetWidth() > 4 && GetHeight() > 4 && Random.Chance(50)) PaintTunnel(Level, Fl, true);

			if (Fl == Terrain.LAVA) PaintTunnel(Level, Terrain.RandomFloor());

			PaintTunnel(Level, Fl == Terrain.DIRT || Fl == Terrain.LAVA ? (Random.Chance(50) ? Terrain.WATER : Terrain.DIRT) : Terrain.RandomFloor());

			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.TUNNEL);

			var Ring = GetConnectionSpace();
			var Floor = Terrain.RandomFloor();
			Painter.Fill(Level, Ring.Left, Ring.Top, 3, 3, Terrain.RandomFloor());
			Painter.Fill(Level, Ring.Left, Ring.Top, 3, 3, Fl == Terrain.LAVA ? (Random.Chance(50) ? Terrain.WATER : Terrain.DIRT) : Floor);
			Painter.Fill(Level, Ring.Left + 1, Ring.Top + 1, 1, 1, Random.Chance(50) ? Terrain.CHASM : Terrain.WALL);
		}

		protected override Rect GetConnectionSpace() {
			if (ConnSpace == null) {
				var C = GetDoorCenter();
				C.X = (int) MathUtils.Clamp(Left + 2, Right - 2, C.X);
				C.Y = (int) MathUtils.Clamp(Top + 2, Bottom - 2, C.Y);
				ConnSpace = new Rect((int) C.X - 1, (int) C.Y - 1, (int) C.X + 1, (int) C.Y + 1);
			}

			return ConnSpace;
		}
	}
}