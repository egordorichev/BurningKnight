using Lens.graphics.animation;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class ZAnimationComponent : AnimationComponent {
		public ZAnimationComponent(string animationName, string layer = null, string tag = null) : base(animationName, layer, tag) {
		}

		public ZAnimationComponent(string animationName, ColorSet set) : base(animationName, set) {
		}

		protected override void CallRender(Vector2 pos, bool shadow) {
			var component = GetComponent<ZComponent>();
			Animation?.Render(pos + new Vector2(0, (shadow ? 0 : -1) * component.Z), Flipped, FlippedVerticaly);
		}
	}
}