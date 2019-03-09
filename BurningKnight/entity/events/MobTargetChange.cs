using Lens.entity;

namespace BurningKnight.entity.events {
	public class MobTargetChange : Event {
		public Entity Mob;
		public Entity Old;
		public Entity New;
	}
}