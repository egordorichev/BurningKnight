using System;
using Lens.entity;
using Lens.entity.component;

namespace BurningKnight.entity.component {
	public class InteractableComponent : Component {
		public Func<Entity, bool> Interact;
		public Action<Entity> OnStart;
		public Action<Entity> OnEnd;

		public InteractableComponent(Func<Entity, bool> interact) {
			Interact = interact;
		}
	}
}