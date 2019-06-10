using System;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util;
using Random = Lens.util.math.Random;

namespace BurningKnight.level.rooms.connection {
	public class RingConnectionRoom : ConnectionRoom {
		public override int GetMinWidth() {
			return 5;
		}

		public override int GetMinHeight() {
			return 5;
		}

		public override void Paint(Level level) {
			Painter.Fill(level, this, 1, Tiles.Pick(Tile.WallA, Tile.Chasm));

			var ring = GetConnectionSpace();
			var b = Random.Chance();
			var d = b && Random.Chance();
			
			PaintTunnel(level, d ? Tiles.Pick(Tile.WallA, Tile.WallB, Tile.Planks, Tiles.RandomFloor(), Tile.FloorD) : Tiles.RandomFloor(), ring, b); 

			if (d && Random.Chance()) {
				PaintTunnel(level, Tiles.RandomNewFloor(), ring);
			}
			
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