using BurningKnight.entity.level;
using Lens.entity;

namespace BurningKnight.entity.events {
	public class TileCollisionEndEvent : Event {
		public Entity Who;
		public Tile Tile;
	}
}