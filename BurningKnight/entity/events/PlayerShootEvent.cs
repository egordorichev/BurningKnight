using BurningKnight.entity.creature.player;
using Lens.entity;

namespace BurningKnight.entity.events {
	public class PlayerShootEvent : Event {
		public Player Player;
		public int Times = 1;
		public bool Accurate;
	}
}