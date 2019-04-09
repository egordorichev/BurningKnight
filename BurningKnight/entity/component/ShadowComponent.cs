using System;
using Lens.entity.component;

namespace BurningKnight.entity.component {
	public class ShadowComponent : Component {
		public Action Callback;
		
		public ShadowComponent(Action render) {
			Callback = render;
		}

		public override void Init() {
			base.Init();
			Entity.AddTag(Tags.HasShadow);
		}
	}
}