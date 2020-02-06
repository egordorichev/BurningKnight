using Lens.entity;

namespace BurningKnight.entity.events {
	public class DealChanceCalculateEvent : Event {
		public float GrannyStartChance;
		public float GrannyChance;
		public float DmStartChance;
		public float DmChance;

		public bool OpenBoth;

		public Entity Who;
	}
}