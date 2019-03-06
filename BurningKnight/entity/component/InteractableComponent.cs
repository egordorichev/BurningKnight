using System;
using Lens.entity;
using Lens.entity.component;

namespace BurningKnight.entity.component {
	public class InteractableComponent : Component {
		public Action<Entity> Interact;

		public InteractableComponent(Action<Entity> interact) {
			Interact = interact;
		}
	}
}