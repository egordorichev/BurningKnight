using System;
using BurningKnight.level.tile;
using Lens.util.math;

namespace BurningKnight.level.rooms.boss {
	public class CollumnsBossRoom : BossRoom {
		protected override void PaintRoom(Level level) {
			var minDim = Math.Min(GetWidth(), GetHeight());
			var circ = Rnd.Chance();

			var left = Left;
			var top = Top;
			var right = Right;
			var bottom = Bottom;

			var a = Tile.WallA;
			var b = a;
			var af = Tiles.Pick(Tile.FloorA, Tile.FloorB, Tile.FloorC, Tile.FloorD);
			var bf = Rnd.Chance() ? af : Tiles.Pick(Tile.FloorA, Tile.FloorB, Tile.FloorC, Tile.FloorD);
			
			var o = Rnd.Chance();
			int pillarInset = Rnd.Int(1, 3);
			int pillarSize = 2;

			int pillarX, pillarY;

			if (Rnd.Int(2) == 0) {
				pillarX = Rnd.Int(left + 1 + pillarInset, right - pillarSize - pillarInset);
				pillarY = top + 1 + pillarInset;
			} else {
				pillarX = left + 1 + pillarInset;
				pillarY = Rnd.Int(top + 1 + pillarInset, bottom - pillarSize - pillarInset);
			}

			if (circ) {
				if (o) {
					Painter.FillEllipse(level, pillarX - 1, pillarY - 1, pillarSize + 2, pillarSize + 2, af);
				}
					
				Painter.FillEllipse(level, pillarX, pillarY, pillarSize, pillarSize, a);
			} else {
				if (o) {
					Painter.Fill(level, pillarX - 1, pillarY - 1, pillarSize + 2, pillarSize + 2, af);
				}

				Painter.Fill(level, pillarX, pillarY, pillarSize, pillarSize, a);
			}

			pillarX = right - (pillarX - left + pillarSize - 1);
			pillarY = bottom - (pillarY - top + pillarSize - 1);

			if (circ) {
				if (o) {
					Painter.FillEllipse(level, pillarX - 1, pillarY - 1, pillarSize + 2, pillarSize + 2, bf);
				}
					
				Painter.FillEllipse(level, pillarX, pillarY, pillarSize, pillarSize, b);
			} else {
				if (o) {
					Painter.Fill(level, pillarX - 1, pillarY - 1, pillarSize + 2, pillarSize + 2, bf);
				}

				Painter.Fill(level, pillarX, pillarY, pillarSize, pillarSize, b);
			}
		}
	}
}