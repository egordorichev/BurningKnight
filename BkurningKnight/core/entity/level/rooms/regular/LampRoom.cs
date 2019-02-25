using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.pool.room;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class LampRoom : RegularRoom {
		public override Void Paint(Level Level) {
			RegularRoom Room = LampRoomPool.Instance.Generate();
			Room.Size = this.Size;
			Room.Left = this.Left;
			Room.Right = this.Right;
			Room.Top = this.Top;
			Room.Bottom = this.Bottom;
			Room.Neighbours = this.Neighbours;
			Room.Connected = this.Connected;
			Room.Paint(Level);

			foreach (LDoor Door in this.Connected.Values()) {
				Door.SetType(LDoor.Type.ENEMY);
			}
		}

		public override int GetMinHeight() {
			return 7;
		}

		public override int GetMinWidth() {
			return 7;
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.ALL) {
				return 2;
			} 

			return 1;
		}

		public override int GetMaxConnections(Connection Side) {
			return 2;
		}
	}
}
