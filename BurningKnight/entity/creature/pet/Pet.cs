using BurningKnight.entity.component;
using Lens.entity;

namespace BurningKnight.entity.creature.pet {
	public class Pet : Creature {
		public Entity Owner;

		public override void AddComponents() {
			base.AddComponents();
			
			RemoveComponent<HealthComponent>();
			RemoveTag(Tags.LevelSave);
		}
	}
}