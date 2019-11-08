using System;
using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.level.walls {
	public class CollumnWall : WallPainter {
		public override void Paint(Level level, RoomDef room, Rect inside) {
			var s = Math.Min(inside.GetWidth(), inside.GetHeight());

			inside = inside.Shrink(s / 3); // Math.Min(s / 2 - 2, s / 4 + Random.Int(0, s / 2)));
			Painter.Fill(level, inside, Tiles.Pick(Tile.Chasm, Tiles.RandomFillWall()));

			if (Random.Chance()) {
				var p = Random.Chance(30);
				var tile = p ? Tiles.RandomFloor() : Tiles.RandomSolid();

				if (tile == Tile.Lava) {
					Painter.Fill(level, inside, 1, Tiles.RandomFloor());
				}
				
				Painter.Fill(level, inside, 1, tile);

				if (p || Random.Chance(70)) {
					var m = false;
					var f = Tiles.RandomFloorOrSpike();

					if (Random.Chance(40)) {
						m = true;

						Painter.Set(level, new Dot(inside.Left, inside.Top + inside.GetHeight() / 2 - 1), f);
						Painter.Set(level, new Dot(inside.Left, inside.Top + inside.GetHeight() / 2), f);
					}

					if (Random.Chance(40)) {
						m = true;
					
						Painter.Set(level, new Dot(inside.Right - 1, inside.Top + inside.GetHeight() / 2 + 1), f);
						Painter.Set(level, new Dot(inside.Right - 1, inside.Top + inside.GetHeight() / 2), f);
					}

					if (Random.Chance(40)) {
						m = true;
					
						Painter.Set(level, new Dot(inside.Left + inside.GetWidth() / 2 - 1, inside.Top), f);
						Painter.Set(level, new Dot(inside.Left + inside.GetWidth() / 2, inside.Top), f);
					}

					if (!m || Random.Chance(40)) {
						Painter.Set(level, new Dot(inside.Left + inside.GetWidth() / 2 + 1, inside.Bottom - 1), f);
						Painter.Set(level, new Dot(inside.Left + inside.GetWidth() / 2, inside.Bottom - 1), f);
					}
				}
			}

			if (Random.Chance(30)) {
				room.PaintTunnel(level, Tiles.RandomNewFloor(), room.GetCenterRect(), true);
				room.PaintTunnel(level, Tiles.RandomNewFloor(), room.GetCenterRect());
			}
		}
	}
}