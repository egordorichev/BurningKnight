using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.walls {
	public class EllipseWalls : WallPainter {
		public override void Paint(Level level, RoomDef room, Rect inside) {
			Painter.Fill(level, room, Tiles.RandomWall());
			Painter.FillEllipse(level, inside, Tiles.RandomFloor());

			var before = Random.Chance();

			if (before) {
				if (Random.Chance()) {
					room.PaintTunnel(level, Tiles.RandomNewFloor(), new Rect(room.GetCenter()), true);
				}
				
				room.PaintTunnel(level, Tiles.RandomNewFloor(), new Rect(room.GetCenter()));
			}

			if (Random.Chance()) {
				Painter.Ellipse(level, inside,Tiles.RandomFloor());
			}
			
			if (!before) {
				if (Random.Chance()) {
					room.PaintTunnel(level, Tiles.RandomNewFloor(), new Rect(room.GetCenter()), true);
				}
				
				room.PaintTunnel(level, Tiles.RandomNewFloor(), new Rect(room.GetCenter()));
			}
		}
	}
}