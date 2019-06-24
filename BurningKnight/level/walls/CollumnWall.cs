using System;
using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Random = Lens.util.math.Random;

namespace BurningKnight.level.walls {
	public class CollumnWall : WallPainter {
		public override void Paint(Level level, RoomDef room, Rect inside) {
			var s = Math.Min(inside.GetWidth(), inside.GetHeight());

			inside = inside.Shrink(s / 3); // Math.Min(s / 2 - 2, s / 4 + Random.Int(0, s / 2)));
			Painter.Fill(level, inside, Tiles.Pick(Tile.Chasm, Tiles.RandomFillWall()));

			if (Random.Chance()) {
				var tile = Tiles.Pick(Tile.Chasm, Tiles.RandomNewWall(), Tile.Lava);
				inside.Shrink(Random.Int(1, 3));

				if (tile == Tile.Lava) {
					Painter.Fill(level, inside, Tiles.RandomFloor());
				}
				
				Painter.Fill(level, inside, tile);
			}

			if (Random.Chance()) {
				room.PaintTunnel(level, Tiles.RandomNewFloor(), room.GetCenterRect(), true);
				room.PaintTunnel(level, Tiles.RandomNewFloor(), room.GetCenterRect());
			}
		}
	}
}