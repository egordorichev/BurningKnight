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

			if (Rnd.Chance()) {
				inside = inside.Shrink(s / 3);
			} else {
				var c = room.GetTileCenter();
				var sz = s / 2;
				inside = new Rect(c.X - sz / 2, c.Y - sz / 2).Resize(sz, sz);
			}
			
			Painter.Fill(level, inside, Tiles.Pick(Tile.WallA, Tile.WallB, Tile.Planks));

			if (Rnd.Chance(80)) {
				var p = Rnd.Chance(30);
				var tile = p ? Tiles.RandomFloor() : Tiles.RandomWall();

				if (tile == Tile.Lava) {
					Painter.Fill(level, inside, 1, Tiles.RandomFloor());
				} else if (tile == Tile.SensingSpikeTmp) {
					tile = Tile.FloorA;	
				}
				
				Painter.Fill(level, inside, 1, tile);

				if (p || Rnd.Chance(70)) {
					var m = false;
					var f = Tiles.RandomFloorOrSpike();
					var i = Rnd.Chance();
					var fi = Rnd.Chance() ? f : Tiles.RandomFloorOrSpike();

					Painter.Fill(level, inside, 2, Tiles.RandomFloor());

					if (Rnd.Chance(40)) {
						m = true;

						if (i) {
							Painter.Set(level, new Dot(inside.Left + 2, inside.Top + inside.GetHeight() / 2), fi);
						}

						Painter.Set(level, new Dot(inside.Left + 1, inside.Top + inside.GetHeight() / 2), f);
					}

					if (Rnd.Chance(40)) {
						m = true;
					
						if (i) {
							Painter.Set(level, new Dot(inside.Right - 3, inside.Top + inside.GetHeight() / 2), fi);
						}
						
						Painter.Set(level, new Dot(inside.Right - 2, inside.Top + inside.GetHeight() / 2), f);
					}

					if (Rnd.Chance(40)) {
						m = true;

						if (i) {
							Painter.Set(level, new Dot(inside.Left + inside.GetWidth() / 2, inside.Top + 2), fi);
						}

						Painter.Set(level, new Dot(inside.Left + inside.GetWidth() / 2, inside.Top + 1), f);
					}

					if (!m || Rnd.Chance(40)) {
						if (i) {
							Painter.Set(level, new Dot(inside.Left + inside.GetWidth() / 2, inside.Bottom - 3), fi);
						}

						Painter.Set(level, new Dot(inside.Left + inside.GetWidth() / 2, inside.Bottom - 2), f);
					}
				}

				var fr = Tiles.RandomFloor();
				Painter.Set(level, new Dot(inside.Left + inside.GetWidth() / 2, inside.Bottom - 1), fr);
				Painter.Set(level, new Dot(inside.Left + inside.GetWidth() / 2, inside.Top), fr);
				Painter.Set(level, new Dot(inside.Left, inside.Top + inside.GetHeight() / 2), fr);
				Painter.Set(level, new Dot(inside.Right - 1, inside.Top + inside.GetHeight() / 2), fr);
			}

			if (Rnd.Chance(30)) {
				if (Rnd.Chance()) {
					room.PaintTunnel(level, Tiles.RandomNewFloor(), room.GetCenterRect(), true);
				}

				room.PaintTunnel(level, Tiles.RandomNewFloor(), room.GetCenterRect());
			}
		}
	}
}
