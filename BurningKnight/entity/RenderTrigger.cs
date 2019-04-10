using System;
using Lens.entity;

namespace BurningKnight.entity {
	public class RenderTrigger : Entity {
		private Action method;
		public Entity Entity;
		
		public RenderTrigger(Entity entity, Action method, int depth) {
			Depth = depth;
			Entity = entity;
			AlwaysActive = true;
			AlwaysVisible = true;

			this.method = method;
		}

		public override void Render() {
			if (Entity.Done) {
				Done = true;
				return;
			}

			method();
		}
	}
}