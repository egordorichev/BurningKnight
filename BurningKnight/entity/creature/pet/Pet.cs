using System;
using BurningKnight.entity.component;
using Lens.entity;

namespace BurningKnight.entity.creature.pet {
	public class Pet : Creature {
		public Entity Owner;
		public Action<float> Controller;

		protected bool Saveable;
		private bool findOwner;

		public override void Update(float dt) {
			base.Update(dt);
			Controller?.Invoke(dt);
		}

		public override void AddComponents() {
			base.AddComponents();

			AlwaysActive = true;
			
			RemoveComponent<HealthComponent>();			
			RemoveComponent<TileInteractionComponent>();

			if (!Saveable) {
				RemoveTag(Tags.LevelSave);
			} else {
				findOwner = true;
			}
		}

		public override void PostInit() {
			base.PostInit();
			Follow();
		}

		protected virtual void Follow() {
			Owner?.GetComponent<FollowerComponent>()?.AddFollower(this);
		}
	}
}