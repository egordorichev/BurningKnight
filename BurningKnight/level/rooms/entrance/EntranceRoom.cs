using BurningKnight.entity.creature.npc;
using BurningKnight.entity.item;
using BurningKnight.level.entities;
using BurningKnight.level.walls;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.entrance {
	public class EntranceRoom : RoomDef {
		protected bool IgnoreEntranceRooms;
		
		public override bool CanConnect(RoomDef R) {
			return base.CanConnect(R) && (IgnoreEntranceRooms || !(R is EntranceRoom || R is ExitRoom));
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
			
			foreach (var door in Connected.Values) {
				door.Type = DoorPlaceholder.Variant.Regular;
			}
			
			Entrance entrance;
			
			level.Area.Add(entrance = new Entrance {
				To = -1
			});

			entrance.Center = GetCenter() * 16;
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