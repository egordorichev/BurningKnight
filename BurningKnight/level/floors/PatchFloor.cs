using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;

namespace BurningKnight.level.floors {
	public class PatchFloor : FloorPainter {
		public override void Paint(Level level, RoomDef room, Rect inside, bool gold) {
			Painter.Fill(level, room, 1, Tiles.RandomFloor());
			
			var w = room.GetWidth() - 2;
			var h = room.GetHeight() - 2;
			var fill = 0.25f + (room.GetWidth() * room.GetHeight()) / 1024f;
			var f = gold ? Tile.FloorD : Tiles.RandomNewFloor();
			
			var patch = Patch.Generate(w, h, fill, 4);

			for (var y = 0; y < room.GetHeight() - 2; y++) {
				for (var x = 0; x < w; x++) {
					if (patch[x + y * w]) {
						level.Set(room.Left + x + 1, room.Top + y + 1, f);						
					}
				}
			}
		}
	}
}