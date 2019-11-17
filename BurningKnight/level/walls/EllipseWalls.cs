using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.walls {
	public class EllipseWalls : WallPainter {
		public override void Paint(Level level, RoomDef room, Rect inside) {
			Painter.Fill(level, room, Tiles.RandomWall());

			var t = Tiles.RandomSolid();

			if (t == Tile.Lava) {
				Painter.Fill(level, inside, Tiles.RandomFloor());
			}

			Painter.Fill(level, inside, t);
			Painter.FillEllipse(level, inside, Tiles.RandomFloor());

			// fixme: breaks custom floors
			
			var before = Rnd.Chance();

			if (before) {
				if (Rnd.Chance()) {
					room.PaintTunnel(level, Tiles.RandomNewFloor(), new Rect(room.GetCenter()), true);
				}
				
				room.PaintTunnel(level, Tiles.RandomNewFloor(), new Rect(room.GetCenter()));
			}

			if (Rnd.Chance()) {
				Painter.Ellipse(level, inside,Tiles.RandomFloor());
			}
			
			if (!before) {
				if (Rnd.Chance()) {
					room.PaintTunnel(level, Tiles.RandomNewFloor(), new Rect(room.GetCenter()), true);
				}
				
				room.PaintTunnel(level, Tiles.RandomNewFloor(), new Rect(room.GetCenter()));
			}
		}
	}
}