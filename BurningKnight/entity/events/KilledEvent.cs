using BurningKnight.entity.creature;
using Lens.entity;

namespace BurningKnight.entity.events {
	public class KilledEvent : Event {
		public Creature Who;
		public Creature KilledBy;
	}
}