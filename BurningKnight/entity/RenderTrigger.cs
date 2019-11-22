using System;
using Lens.entity;
using Lens.util;

namespace BurningKnight.entity {
	public class RenderTrigger : Entity {
		private Action method;
		public Entity Entity;
		
		
		public RenderTrigger(Action method, int depth) {
			Depth = depth;
			AlwaysActive = true;
			AlwaysVisible = true;

			this.method = method;
		}
		
		public RenderTrigger(Entity entity, Action method, int depth) {
			Depth = depth;
			Entity = entity;
			AlwaysActive = true;
			AlwaysVisible = true;

			this.method = method;
		}

		public override void Render() {
			if (Entity != null && Entity.Done) {
				Done = true;
				return;
			}

			method();
		}
	}
}