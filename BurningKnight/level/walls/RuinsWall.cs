using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.walls {
	public class RuinsWall : PatchWall {
		public override void Paint(Level level, RoomDef room, Rect inside) {
			var fill = 0.25f + (room.GetWidth() * room.GetHeight()) / 2048f;
			var t = Random.Chance(20) ? Tile.Lava : Tiles.RandomFloorOrSpike();
			
			Setup(level, room, fill, 0, true);
			CleanDiagonalEdges(room);

			for (var i = room.Top + 1; i < room.Bottom; i++) {
				for (var j = room.Left + 1; j < room.Right; j++) {
					if (Patch[ToIndex(room, j, i)]) {
						level.Set(j, i, t);
					}
				}
			}
		}
	}
}