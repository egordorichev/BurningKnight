using BurningKnight.entity.room;
using BurningKnight.entity.room.input;
using BurningKnight.level.tile;
using Lens.util.math;

namespace BurningKnight.level.rooms.trap {
	public class FollowingSpikeBallRoom : TrapRoom {
		public override void Paint(Level level) {
			Painter.Fill(level, this, 1, Tile.SpikeOffTmp);

			if (Rnd.Chance()) {
				var c = GetTileCenter();
				Painter.Set(level, c, Tiles.RandomFloor());

				level.Area.Add(new Button {
					Position = c * 16
				});
			}
		}

		public override void ModifyRoom(Room room) {
			base.ModifyRoom(room);
			room.AddController("bk:following_spike_ball");
		}
	}
}