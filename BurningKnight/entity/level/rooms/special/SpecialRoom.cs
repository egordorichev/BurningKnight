using BurningKnight.entity.pool.room;

namespace BurningKnight.entity.level.rooms.special {
	public class SpecialRoomDef : RoomDef {
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
			if (Side == Connection.All) return 1;

			return 0;
		}

		public static void Init() {
			SpecialRoomPool.Instance.Reset();
		}

		public static SpecialRoomDef Create() {
			return SpecialRoomPool.Instance.Generate();
		}
	}
}