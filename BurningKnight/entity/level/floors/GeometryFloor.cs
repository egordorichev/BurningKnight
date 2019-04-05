using BurningKnight.entity.level.rooms;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.entity.level.floors {
	public class GeometryFloor : FloorPainter {
		public override void Paint(Level level, RoomDef room, Rect inside) {
			Painter.Fill(level, inside, Tiles.RandomFloor());

			if (Random.Chance()) {
				Painter.FillEllipse(level, inside, Tiles.RandomNewFloor());
			}

			if (Random.Chance(20)) {
				return;
			}
			
			inside = inside.Shrink(Random.Int(1, 3));

			if (Random.Chance()) {
				Painter.Fill(level, inside, Tiles.RandomNewFloor());
			} else {
				Painter.Fill(level, inside, Tiles.RandomNewFloor());
			}
			
			if (Random.Chance(40)) {
				return;
			}
			
			inside = inside.Shrink(Random.Int(1, 3));

			if (Random.Chance()) {
				Painter.Fill(level, inside, Tiles.RandomNewFloor());
			} else {
				Painter.Fill(level, inside, Tiles.RandomNewFloor());
			}
		}
	}
}