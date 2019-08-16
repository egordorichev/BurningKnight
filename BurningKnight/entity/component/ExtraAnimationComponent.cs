using Lens.graphics.animation;

namespace BurningKnight.entity.component {
	public class ExtraAnimationComponent : AnimationComponent {
		public ExtraAnimationComponent(string animationName, string layer = null, string tag = null) : base(animationName, layer, tag) {
		}

		public ExtraAnimationComponent(string animationName, ColorSet set) : base(animationName, set) {
		}
	}
}