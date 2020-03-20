using BurningKnight.entity.room;
using BurningKnight.level.rooms;
using Lens.entity;

namespace BurningKnight.entity.events {
	public class RoomChangedEvent : Event {
		public Entity Who;
		public Room Old;
		public Room New;
		public bool WasDiscovered;
		public bool JustDiscovered;
	}
}