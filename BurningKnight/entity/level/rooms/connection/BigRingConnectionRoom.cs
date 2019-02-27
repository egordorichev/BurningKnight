using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.connection {
	public class BigRingConnectionRoomDef : TunnelRoomDef {
		private Rect ConnSpace;

		public override int GetMinWidth() {
			return Math.Max(7, base.GetMinWidth());
		}

		public override int GetMinHeight() {
			return Math.Max(7, base.GetMinHeight());
		}

		public override void Paint(Level Level) {
			Painter.Fill(Level, this, Terrain.WALL);

			if (Random.Chance(50)) Painter.Fill(Level, this, 1, Terrain.CHASM);

			var Ring = GetConnectionSpace();
			var Floor = Terrain.RandomFloor();
			Painter.Fill(Level, Ring.Left - 1, Ring.Top - 1, 5, 5, Floor);

			if (Random.Chance(50)) {
				Painter.Fill(Level, Ring.Left + 1, Ring.Top + 1, 1, 1, Random.Chance(50) ? Terrain.CHASM : Terrain.WALL);

				if (Random.Chance(50)) Painter.Fill(Level, Ring.Left, Ring.Top, 3, 3, Random.Chance(50) ? Terrain.CHASM : Terrain.WALL);
			}
			else {
				Painter.Fill(Level, Ring.Left, Ring.Top, 3, 3, Random.Chance(50) ? Terrain.CHASM : Terrain.WALL);
			}


			PaintTunnel(Level, Terrain.RandomFloor());
			Painter.Fill(Level, Ring.Left, Ring.Top, 3, 3, Terrain.RandomFloor());
			Painter.Fill(Level, Ring.Left + 1, Ring.Top + 1, 1, 1, Random.Chance(50) ? Terrain.CHASM : Terrain.WALL);

			foreach (LDoor D in Connected.Values()) D.SetType(LDoor.Type.TUNNEL);
		}

		protected override Rect GetConnectionSpace() {
			if (ConnSpace == null) {
				var C = GetDoorCenter();
				C.X = (int) MathUtils.Clamp(Left + 3, Right - 3, C.X);
				C.Y = (int) MathUtils.Clamp(Top + 3, Bottom - 3, C.Y);
				ConnSpace = new Rect((int) C.X - 1, (int) C.Y - 1, (int) C.X + 1, (int) C.Y + 1);
			}

			return ConnSpace;
		}
	}
}