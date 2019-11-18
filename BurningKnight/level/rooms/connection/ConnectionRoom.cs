using BurningKnight.entity.room.controllable.spikes;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.connection {
	public class ConnectionRoom : RoomDef {
		public override int GetMinWidth() {
			return 3;
		}

		public override int GetMinHeight() {
			return 3;
		}

		public override int GetMaxWidth() {
			return 10;
		}

		public override int GetMaxHeight() {
			return 10;
		}
		
		protected Rect GenerateSpot() {
			var r = Rnd.Float();

			if (r < 0.33f) {
				return GetConnectionSpace();
			} else if (r < 0.66f) {
				return GetCenterRect();
			}
			
			return new Rect(new Dot(Rnd.Int(Left + 2, Right - 2), Rnd.Int(Top + 2, Bottom - 2)));
		}

		public override void Paint(Level level) {
			
		}

		public override int GetMaxConnections(Connection Side) {
			if (Side == Connection.All) return 16;
			return 4;
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.All) return 2;
			return 0;
		}
	}
}