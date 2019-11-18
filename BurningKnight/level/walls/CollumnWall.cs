using System;
using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.walls {
	public class CollumnWall : WallPainter {
		public override void Paint(Level level, RoomDef room, Rect inside) {
			var s = Math.Min(inside.GetWidth(), inside.GetHeight());

			inside = inside.Shrink(s / 3); // Math.Min(s / 2 - 2, s / 4 + Random.Int(0, s / 2)));
			Painter.Fill(level, inside, Tiles.Pick(Tile.Chasm, Tiles.RandomFillWall()));

			if (Rnd.Chance()) {
				var p = Rnd.Chance(30);
				var tile = p ? Tiles.RandomFloor() : Tiles.RandomSolid();

				if (tile == Tile.Lava) {
					Painter.Fill(level, inside, 1, Tiles.RandomFloor());
				}
				
				Painter.Fill(level, inside, 1, tile);

				if (p || Rnd.Chance(70)) {
					var m = false;
					var f = Tiles.RandomFloorOrSpike();

					if (Rnd.Chance(40)) {
						m = true;

						Painter.Set(level, new Dot(inside.Left, inside.Top + inside.GetHeight() / 2 - 1), f);
						Painter.Set(level, new Dot(inside.Left, inside.Top + inside.GetHeight() / 2), f);
					}

					if (Rnd.Chance(40)) {
						m = true;
					
						Painter.Set(level, new Dot(inside.Right - 1, inside.Top + inside.GetHeight() / 2 + 1), f);
						Painter.Set(level, new Dot(inside.Right - 1, inside.Top + inside.GetHeight() / 2), f);
					}

					if (Rnd.Chance(40)) {
						m = true;
					
						Painter.Set(level, new Dot(inside.Left + inside.GetWidth() / 2 - 1, inside.Top), f);
						Painter.Set(level, new Dot(inside.Left + inside.GetWidth() / 2, inside.Top), f);
					}

					if (!m || Rnd.Chance(40)) {
						Painter.Set(level, new Dot(inside.Left + inside.GetWidth() / 2 + 1, inside.Bottom - 1), f);
						Painter.Set(level, new Dot(inside.Left + inside.GetWidth() / 2, inside.Bottom - 1), f);
					}
				}
			}

			if (Rnd.Chance(30)) {
				room.PaintTunnel(level, Tiles.RandomNewFloor(), room.GetCenterRect(), true);
				room.PaintTunnel(level, Tiles.RandomNewFloor(), room.GetCenterRect());
			}
		}
	}
}