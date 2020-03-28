using BurningKnight.entity.room;
using BurningKnight.level.rooms;
using BurningKnight.state;
using Lens.assets;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class RevealMapUse : ItemUse {
		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);
			
			var level = Run.Level;
			Audio.PlaySfx("item_map");
			
			foreach (var e in entity.Area.Tagged[Tags.Room]) {
				var room = (Room) e;
				
				if (room.Type == RoomType.DarkMarket || room.Type == RoomType.Hidden) {
					continue;
				}

				if (room.Type != RoomType.Secret && room.Type != RoomType.Granny && room.Type != RoomType.OldMan) {
					for (var y = room.MapY; y < room.MapY + room.MapH; y++) {
						for (var x = room.MapX; x < room.MapX + room.MapW; x++) {
							level.Explored[level.ToIndex(x, y)] = true;
						}	
					}
				}
			}
		}
	}
}