using System;
using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Random = Lens.util.math.Random;

namespace BurningKnight.level.walls {
	public class CollumsWall : WallPainter {
		public override void Paint(Level level, RoomDef room, Rect inside) {
			base.Paint(level, room, inside);

			var minDim = Math.Min(room.GetWidth(), room.GetHeight());
			var circ = Random.Chance(50);

			var left = room.Left;
			var top = room.Top;
			var right = room.Right;
			var bottom = room.Bottom;

			var same = Random.Chance();
			var a = Tiles.Pick(Tile.Chasm, Tile.WallA, Tile.Planks, Tile.SensingSpikeTmp, Tile.Lava);
			var b = same ? a : Tiles.Pick(Tile.Chasm, Tile.WallA, Tile.Planks, Tile.SensingSpikeTmp, Tile.Lava);
			var af = Tiles.Pick(Tile.FloorA, Tile.FloorB, Tile.FloorC, Tile.FloorD);
			var bf = a == b ? af : Tiles.Pick(Tile.FloorA, Tile.FloorB, Tile.FloorC, Tile.FloorD);
			
			var o = Random.Chance();

			if (minDim == 7 || Random.Int(2) == 0) {
				int pillarInset = minDim >= 11 ? 2 : 1;
				int pillarSize = ((minDim - 3) / 2) - pillarInset;

				int pillarX, pillarY;

				if (Random.Int(2) == 0) {
					pillarX = Random.Int(left + 1 + pillarInset, right - pillarSize - pillarInset);
					pillarY = top + 1 + pillarInset;
				} else {
					pillarX = left + 1 + pillarInset;
					pillarY = Random.Int(top + 1 + pillarInset, bottom - pillarSize - pillarInset);
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
			} else {
				int pillarInset = minDim >= 12 ? 2 : 1;
				int pillarSize = (minDim - 6) / (pillarInset + 1);

				float xSpaces = room.GetWidth() - 2 * pillarInset - pillarSize - 2;
				float ySpaces = room.GetHeight() - 2 * pillarInset - pillarSize - 2;
				float minSpaces = Math.Min(xSpaces, ySpaces);

				float percentSkew = (float) (Math.Round(Random.Float() * minSpaces) / minSpaces);

				if (circ) {
					if (o) {
						Painter.FillEllipse(level, left + pillarInset + (int) Math.Round(percentSkew * xSpaces), top + pillarInset,
							pillarSize + 2, pillarSize + 2, af);

						Painter.FillEllipse(level, right - pillarSize - pillarInset - 1,
							top + pillarInset + (int) Math.Round(percentSkew * ySpaces), pillarSize + 2, pillarSize + 2, bf);

						Painter.FillEllipse(level, right - pillarSize - pillarInset - (int) Math.Round(percentSkew * xSpaces) - 1,
							bottom - pillarSize - pillarInset - 1, pillarSize + 2, pillarSize + 2, af);

						Painter.FillEllipse(level, left + pillarInset,
							bottom - pillarSize - pillarInset - (int) Math.Round(percentSkew * ySpaces) - 1, pillarSize + 2, pillarSize + 2,
							bf);
					}
					
					Painter.FillEllipse(level, left + 1 + pillarInset + (int) Math.Round(percentSkew * xSpaces), top + 1 + pillarInset,
						pillarSize, pillarSize, a);

					Painter.FillEllipse(level, right - pillarSize - pillarInset,
						top + 1 + pillarInset + (int) Math.Round(percentSkew * ySpaces), pillarSize, pillarSize, b);

					Painter.FillEllipse(level, right - pillarSize - pillarInset - (int) Math.Round(percentSkew * xSpaces),
						bottom - pillarSize - pillarInset, pillarSize, pillarSize, a);

					Painter.FillEllipse(level, left + 1 + pillarInset,
						bottom - pillarSize - pillarInset - (int) Math.Round(percentSkew * ySpaces), pillarSize, pillarSize,
						b);
				} else {
					if (o) {
						Painter.Fill(level, left + pillarInset + (int) Math.Round(percentSkew * xSpaces), top + pillarInset,
							pillarSize + 2, pillarSize + 2, af);

						Painter.Fill(level, right - pillarSize - pillarInset - 1,
							top + pillarInset + (int) Math.Round(percentSkew * ySpaces), pillarSize + 2, pillarSize + 2, bf);

						Painter.Fill(level, right - pillarSize - pillarInset - (int) Math.Round(percentSkew * xSpaces) - 1,
							bottom - pillarSize - pillarInset - 1, pillarSize + 2, pillarSize + 2, af);

						Painter.Fill(level, left + pillarInset,
							bottom - pillarSize - pillarInset - (int) Math.Round(percentSkew * ySpaces) - 1, pillarSize + 2, pillarSize + 2,
							bf);
					}
					
					Painter.Fill(level, left + 1 + pillarInset + (int) Math.Round(percentSkew * xSpaces), top + 1 + pillarInset,
						pillarSize, pillarSize, a);

					Painter.Fill(level, right - pillarSize - pillarInset,
						top + 1 + pillarInset + (int) Math.Round(percentSkew * ySpaces), pillarSize, pillarSize, b);

					Painter.Fill(level, right - pillarSize - pillarInset - (int) Math.Round(percentSkew * xSpaces),
						bottom - pillarSize - pillarInset, pillarSize, pillarSize, a);

					Painter.Fill(level, left + 1 + pillarInset,
						bottom - pillarSize - pillarInset - (int) Math.Round(percentSkew * ySpaces), pillarSize, pillarSize,
						b);
				}
			}
		}
	}
}