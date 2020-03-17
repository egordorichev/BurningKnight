using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.walls.library {
	public class TeleportSplitWall : LibraryWallPainter {
		public override void Paint(Level level, RoomDef room, Rect inside) {
			base.Paint(level, room, inside);
			var t = Tiles.Pick(Tile.WallA, Tile.Planks);

			if (Rnd.Chance()) {
				Painter.DrawLine(level, new Dot(room.Left + room.GetWidth() / 2, room.Top + 1), new Dot(room.Left + room.GetWidth() / 2, room.Bottom - 1), t);
				PlaceTeleporter(level, new Dot(room.Left + room.GetWidth() / 4, Rnd.Int(room.Top + 3, room.Bottom - 3)), new Dot(room.Left + room.GetWidth() / 4 * 3, Rnd.Int(room.Top + 3, room.Bottom - 3)));
			} else {
				Painter.DrawLine(level, new Dot(room.Left + 1, room.Top + room.GetHeight() / 2), new Dot(room.Right - 1, room.Top + room.GetHeight() / 2), t);
				PlaceTeleporter(level, new Dot(Rnd.Int(room.Left + 3, room.Right - 3), room.Top + room.GetHeight() / 4), new Dot(Rnd.Int(room.Left + 3, room.Right - 3), room.Top + room.GetHeight() / 4 * 3));	
			}
		}
	}
}