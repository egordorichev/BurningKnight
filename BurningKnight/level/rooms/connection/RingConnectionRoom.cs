using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util;

namespace BurningKnight.level.rooms.connection {
	public class RingConnectionRoom : ConnectionRoom {
		public override int GetMinWidth() {
			return 5;
		}

		public override int GetMinHeight() {
			return 5;
		}

		public override void Paint(Level level) {
			var t = Tiles.Pick(Tile.WallA, Tile.Chasm, Tile.Lava);
			
			Painter.Fill(level, this, 1, t);
			PaintTunnel(level, t == Tile.Lava ? Tiles.Pick(Tile.Dirt, Tile.Water) : Tiles.RandomFloor(), GetConnectionSpace());

			var ring = GetConnectionSpace();

			Painter.Fill(level, ring.Left - 1, ring.Top - 1, 3, 3, Tiles.RandomFloor());
			Painter.Set(level, ring.Left, ring.Top, Tiles.Pick(Tile.Chasm, Tile.WallA, Tile.WallB, Tile.Planks));
		}

		private Rect space;

		public override Rect GetConnectionSpace() {
			if (space == null) {
				space = base.GetConnectionSpace();
				space.Left = (int) MathUtils.Clamp(Left + 2, Right - 2, space.Left);
				space.Top = (int) MathUtils.Clamp(Top + 2, Bottom - 2, space.Top);
				space.Right = space.Left + 1;
				space.Bottom = space.Top + 1;
			}

			return space;
		}
	}
}