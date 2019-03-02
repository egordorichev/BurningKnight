using System;
using Lens.entity;

namespace BurningKnight.entity {
	public class RenderTrigger : Entity {
		private Func<bool> method;
		
		public RenderTrigger(Func<bool> method, int depth) {
			Depth = depth;
			this.method = method;

			AlwaysVisible = true;
		}

		public override void Render() {
			method();
		}
	}
}