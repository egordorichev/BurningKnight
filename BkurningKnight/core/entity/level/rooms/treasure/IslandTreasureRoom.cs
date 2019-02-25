using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.treasure {
	public class IslandTreasureRoom : TreasureRoom {
		private Rect ConnSpace;

		public override Void Paint(Level Level) {
			base.Paint(Level);
			Painter.Fill(Level, this, 1, Terrain.FLOOR_D);
			Painter.Fill(Level, this, 1, (Random.Chance(50) ? Terrain.CHASM : Terrain.WALL));
			PaintTunnel(Level, Random.Chance(30) ? Terrain.FLOOR_D : Terrain.RandomFloor());
			Rect Connetion = this.GetConnectionSpace();
			Painter.Fill(Level, Connetion.Left - 1, Connetion.Top - 1, 5, 5, Random.Chance(30) ? Terrain.FLOOR_D : Terrain.RandomFloor());
			PlaceChest(GetCenter());
		}

		protected override Point GetDoorCenter() {
			return GetCenter();
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
