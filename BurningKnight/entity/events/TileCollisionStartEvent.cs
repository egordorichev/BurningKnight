using BurningKnight.entity.level;
using BurningKnight.entity.level.tile;
using Lens.entity;

namespace BurningKnight.entity.events {
	public class TileCollisionStartEvent : Event {
		public Entity Who;
		public Tile Tile;
	}
}