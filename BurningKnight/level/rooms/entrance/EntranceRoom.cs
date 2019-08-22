using BurningKnight.level.entities;
using BurningKnight.level.walls;
using BurningKnight.state;
using Lens.entity;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.entrance {
	public class EntranceRoom : RoomDef {
		protected bool IgnoreEntranceRooms;
		public bool Exit;
		
		public override bool CanConnect(RoomDef R) {
			return base.CanConnect(R) && (IgnoreEntranceRooms || !(R is EntranceRoom));
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.All) return 1;
			return 0;
		}

		public override int GetMaxConnections(Connection Side) {
			if (Side == Connection.All) return 16;
			return 4;
		}

		public override void Paint(Level level) {
			WallRegistry.Paint(level, this, EntranceWallPool.Instance);
			Place(level, GetTileCenter());
		}

		protected virtual void Place(Level level, Vector2 where) {
			var prop = Exit ? (Entity) new Exit {
				To = Run.Depth + 1
			} : new Entrance {
				To = -1
			};

			level.Area.Add(prop);
			prop.Center = (where + new Vector2(8)) * 16;
		}

		public override int GetMinWidth() {
			return 7;
		}

		public override int GetMinHeight() {
			return 7;
		}

		public override int GetMaxWidth() {
			return 12;
		}

		public override int GetMaxHeight() {
			return 12;
		}
	}
}