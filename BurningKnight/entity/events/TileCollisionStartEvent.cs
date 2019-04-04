using BurningKnight.entity.level;
using Lens.entity;

namespace BurningKnight.entity.events {
	public class TileCollisionStartEvent : Event {
		public Entity Who;
		public Tile Tile;
	}
}