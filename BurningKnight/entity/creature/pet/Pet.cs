using BurningKnight.entity.component;
using Lens.entity;

namespace BurningKnight.entity.creature.pet {
	public class Pet : Creature {
		public Entity Owner;

		public override void AddComponents() {
			base.AddComponents();

			AlwaysActive = true;
			
			RemoveComponent<HealthComponent>();			
			RemoveComponent<TileInteractionComponent>();

			RemoveTag(Tags.LevelSave);
		}

		public override void PostInit() {
			base.PostInit();
			Follow();
		}

		protected virtual void Follow() {
			Owner.GetComponent<FollowerComponent>().AddFollower(this);
		}
	}
}