using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.floors {
	public class GeometryFloor : FloorPainter {
		public override void Paint(Level level, RoomDef room, Rect inside, bool gold) {
			Painter.Fill(level, inside, Tiles.RandomFloor(gold));

			if (Rnd.Chance()) {
				Painter.FillEllipse(level, inside, gold && Rnd.Chance(30) ? Tile.FloorD : Tiles.RandomNewFloor());
			}

			if (Rnd.Chance(20)) {
				return;
			}
			
			inside = inside.Shrink(Rnd.Int(1, 3));

			if (Rnd.Chance()) {
				Painter.Fill(level, inside, gold ? Tile.FloorD : Tiles.RandomNewFloor());
			} else {
				Painter.Fill(level, inside, gold ? Tile.FloorD : Tiles.RandomNewFloor());
			}
			
			if (Rnd.Chance(40)) {
				return;
			}
			
			inside = inside.Shrink(Rnd.Int(1, 3));

			if (Rnd.Chance()) {
				Painter.Fill(level, inside, gold && Rnd.Chance(30) ? Tile.FloorD : Tiles.RandomNewFloor());
			} else {
				Painter.Fill(level, inside, gold && Rnd.Chance(30) ? Tile.FloorD : Tiles.RandomNewFloor());
			}
		}
	}
}