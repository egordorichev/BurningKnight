using System;
using Lens.entity;
using Lens.entity.component;

namespace BurningKnight.entity.component {
	public class InteractableComponent : Component {
		public Func<Entity, bool> Interact;
		public Action<Entity> OnStart;
		public Action<Entity> OnEnd;
		public Entity CurrentlyInteracting;
		public float OutlineAlpha;

		public InteractableComponent(Func<Entity, bool> interact) {
			Interact = interact;
		}

		public override void Update(float dt) {
			base.Update(dt);

			OutlineAlpha += ((CurrentlyInteracting == null ? 0 : 1) - OutlineAlpha) * dt * 8;
		}
	}
}