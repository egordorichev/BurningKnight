using BurningKnight.entity.room;
using BurningKnight.level.tile;

namespace BurningKnight.level.rooms.trap {
	public class ShiftingWallsRoom : TrapRoom {
		public override void Paint(Level level) {
			Painter.Fill(level, this, 2, Tile.Piston);
			Painter.Fill(level, this, 3, Tile.PistonDown);
		}

		public override void ModifyRoom(Room room) {
			base.ModifyRoom(room);
			room.AddController("bk:timed_piston_switch");
		}
	}
}