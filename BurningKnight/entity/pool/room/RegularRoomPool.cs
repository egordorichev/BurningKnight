using BurningKnight.entity.level.rooms.regular;
using BurningKnight.state;

namespace BurningKnight.entity.pool.room {
	public class RegularRoomPool : Pool<RegularRoom> {
		public static RegularRoomPool Instance = new RegularRoomPool();

		public RegularRoomPool() {
			Add(typeof(RegularRoom), 3);
		}
	}
}