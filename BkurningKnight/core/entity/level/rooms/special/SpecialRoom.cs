using BurningKnight.core.entity.pool.room;

namespace BurningKnight.core.entity.level.rooms.special {
	public class SpecialRoom : Room {
		public override int GetMinWidth() {
			return 8;
		}

		public int GetMaxWidth() {
			return 14;
		}

		public override int GetMinHeight() {
			return 8;
		}

		public int GetMaxHeight() {
			return 14;
		}

		public override int GetMaxConnections(Connection Side) {
			return 1;
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.ALL) {
				return 1;
			} 

			return 0;
		}

		public static Void Init() {
			SpecialRoomPool.Instance.Reset();
		}

		public static SpecialRoom Create() {
			return SpecialRoomPool.Instance.Generate();
		}
	}
}
