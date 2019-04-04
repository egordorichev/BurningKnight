using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using Lens.entity;

namespace BurningKnight.entity.events {
	public class WeaponSwappedEvent : Event {
		public Player Who;
		public Item Current;
		public Item Old;
	}
}