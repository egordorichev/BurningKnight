using BurningKnight.entity.bomb;
using Lens.entity;

namespace BurningKnight.entity.events {
	public class BombPlacedEvent : Event {
		public Bomb Bomb;
		public Entity Owner;
	}
}