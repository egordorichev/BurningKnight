using System;
using BurningKnight.entity.level.rooms;
using BurningKnight.util.geometry;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.level.walls {
	public class CollumnWall : WallPainter {
		public override void Paint(Level level, RoomDef room, Rect inside) {
			inside = inside.Shrink(Math.Min(inside.GetWidth() / 2 - 1, inside.GetWidth() / 4 + Random.Int(0, inside.GetWidth() / 2)));
			Painter.Fill(level, inside, Tiles.Pick(Tile.Chasm, Tiles.RandomWall()));

			if (Random.Chance()) {
				var tile = Tiles.Pick(Tile.Chasm, Tiles.RandomNewWall(), Tile.Lava);
				inside.Shrink(Random.Int(1, 3));

				if (tile == Tile.Lava) {
					Painter.Fill(level, inside, Tiles.RandomFloor());
				}
				
				Painter.Fill(level, inside, tile);
			}

			if (Random.Chance()) {
				room.PaintTunnel(level, Tiles.RandomNewFloor(), new Rect(room.GetCenter()), true);
				room.PaintTunnel(level, Tiles.RandomNewFloor(), new Rect(room.GetCenter()));
			}
		}
	}
}