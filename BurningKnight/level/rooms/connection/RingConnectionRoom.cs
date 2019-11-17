using System;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.level.rooms.connection {
	public class RingConnectionRoom : ConnectionRoom {
		public override int GetMinWidth() {
			return 5;
		}

		public override int GetMinHeight() {
			return 5;
		}

		public override void Paint(Level level) {
			Painter.Fill(level, this, Tile.WallA);
			Painter.Fill(level, this, 1, Tiles.RandomSolid());

			var ring = GetConnectionSpace();
			var b = Rnd.Chance();
			var d = b && Rnd.Chance();
			
			PaintTunnel(level, d ? Tiles.Pick(Tiles.RandomSolid(), Tiles.RandomFloor(), Tile.FloorD) : Tiles.RandomFloorOrSpike(), ring, b); 

			if (d || Rnd.Chance()) {
				PaintTunnel(level, Tiles.RandomFloorOrSpike(), ring);
			}
			
			Painter.Fill(level, ring.Left - 1, ring.Top - 1, 3, 3, Tiles.RandomFloorOrSpike());
			Painter.Set(level, ring.Left, ring.Top, Tiles.RandomSolid());
		}

		private Rect space;

		public override Rect GetConnectionSpace() {
			if (space == null) {
				space = GenerateSpot();
				space.Left = (int) MathUtils.Clamp(Left + 2, Right - 2, space.Left);
				space.Top = (int) MathUtils.Clamp(Top + 2, Bottom - 2, space.Top);
				space.Right = space.Left + 1;
				space.Bottom = space.Top + 1;
			}

			return space;
		}
	}
}