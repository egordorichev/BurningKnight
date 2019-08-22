using BurningKnight.entity.room;
using BurningKnight.level.tile;

namespace BurningKnight.level.rooms.trap {
	public class FollowingSpikeBallRoom : TrapRoom {
		public override void Paint(Level level) {
			Painter.Fill(level, this, 1, Tile.SpikeTmp);
		}

		public override void ModifyRoom(Room room) {
			base.ModifyRoom(room);
			room.AddController("bk:following_spike_ball");
		}
	}
}