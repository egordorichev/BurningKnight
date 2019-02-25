using BurningKnight.entity.creature.mob;
using BurningKnight.entity.item.entity;

namespace BurningKnight.entity.level.entities {
	public class DoorListener : Entity {
		public Door Door;

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is BombEntity || Entity is Mob) return true;

			return false;
		}
	}
}