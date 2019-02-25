using BurningKnight.entity.level.features;
using BurningKnight.entity.pool.room;

namespace BurningKnight.entity.level.rooms.regular {
	public class LampRoom : RegularRoom {
		public override void Paint(Level Level) {
			var Room = LampRoomPool.Instance.Generate();
			Room.Size = this.Size;
			Room.Left = Left;
			Room.Right = Right;
			Room.Top = Top;
			Room.Bottom = Bottom;
			Room.Neighbours = Neighbours;
			Room.Connected = Connected;
			Room.Paint(Level);

			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.ENEMY);
		}

		public override int GetMinHeight() {
			return 7;
		}

		public override int GetMinWidth() {
			return 7;
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.ALL) return 2;

			return 1;
		}

		public override int GetMaxConnections(Connection Side) {
			return 2;
		}
	}
}