using System.Collections.Generic;
using BurningKnight.entity.events;
using BurningKnight.entity.room;
using BurningKnight.level.rooms;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class DiscoverSideRoomsUse : ItemUse {
		public override bool HandleEvent(Event e) {
			if (e is RoomChangedEvent rce) {
				if (rce.New == null) {
					return base.HandleEvent(e);
				}

				var discovered = new List<Room>() {
					rce.Old
				};
				
				foreach (var d in rce.New.Doors) {
					if (d.Rooms[0] != null && d.Rooms[1] != null) {
						Room room;
						
						if (d.Rooms[0] == rce.New) {
							room = d.Rooms[1];
						} else {
							room = d.Rooms[0];
						}

						if (room.Type == RoomType.Secret || room.Type == RoomType.DarkMarket || room.Type == RoomType.Hidden) {
							continue;
						}

						if (!discovered.Contains(room)) {
							discovered.Add(room);
							room.Discover();
						}
					}
				}
			}

			return base.HandleEvent(e);
		}
	}
}