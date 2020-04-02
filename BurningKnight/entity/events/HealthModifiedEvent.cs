using Lens.entity;

namespace BurningKnight.entity.events {
	public class HealthModifiedEvent : Event {
		public float Amount;
		public Entity From;
		public Entity Who;
		public bool Default = true;
		public DamageType Type = DamageType.Regular;
		public HealthType HealthType;
		public bool PressedForBomb; // fixme: should not flash and drop blood if this is true
	}
}