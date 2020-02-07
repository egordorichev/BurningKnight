using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.walls {
	public class PlatformWall : WallPainter {
		public override void Paint(Level level, RoomDef room, Rect inside) {
			Painter.Fill(level, room, 1, Tiles.Pick(Tile.Chasm, Tile.SensingSpikeTmp));

			var f = Tiles.RandomFloor();
			Painter.Fill(level, room, Rnd.Int(2, 4), f);
			
			room.PaintTunnel(level, Rnd.Chance(30) ? f : Tiles.RandomFloorOrSpike(), room.GetCenterRect());
		}
	}
}