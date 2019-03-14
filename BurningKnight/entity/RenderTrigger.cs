using System;
using Lens.entity;

namespace BurningKnight.entity {
	public class RenderTrigger : Entity {
		private Action method;
		
		public RenderTrigger(Action method, int depth) {
			Depth = depth;
			this.method = method;

			AlwaysVisible = true;
		}

		public override void Render() {
			method();
		}
	}
}