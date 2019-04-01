using BurningKnight.entity.level.entities;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.level.rooms.entrance {
	public class EntranceRoom : RoomDef {
		public override bool CanConnect(RoomDef R) {
			return base.CanConnect(R) && !(R is EntranceRoom);
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.All) return 1;
			return 0;
		}

		public override int GetMaxConnections(Connection Side) {
			if (Side == Connection.All) return 16;
			return 4;
		}
	}
}