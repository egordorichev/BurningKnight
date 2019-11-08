using System;
using Lens.entity.component;

namespace BurningKnight.entity.component {
	public class ZComponent : Component {
		public float Z;
		public bool Float;

		private float t;

		public override void Update(float dt) {
			base.Update(dt);

			if (Float) {
				t += dt;
				Z = 1.5f + (float) Math.Sin(t * 5f) * 1.5f;
			}
		}
	}
}