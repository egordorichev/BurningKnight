using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.treasure {
	public class IslandTreasureRoom : TreasureRoom {
		private Rect ConnSpace;

		public override void Paint(Level Level) {
			base.Paint(Level);
			Painter.Fill(Level, this, 1, Terrain.FLOOR_D);
			Painter.Fill(Level, this, 1, Random.Chance(50) ? Terrain.CHASM : Terrain.WALL);
			PaintTunnel(Level, Random.Chance(30) ? Terrain.FLOOR_D : Terrain.RandomFloor());
			var Connetion = GetConnectionSpace();
			Painter.Fill(Level, Connetion.Left - 1, Connetion.Top - 1, 5, 5, Random.Chance(30) ? Terrain.FLOOR_D : Terrain.RandomFloor());
			PlaceChest(GetCenter());
		}

		protected override Point GetDoorCenter() {
			return GetCenter();
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