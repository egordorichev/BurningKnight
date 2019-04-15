using Lens.graphics;
using Lens.graphics.animation;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class WallAnimationComponent : AnimationComponent {
		public float Angle;
		
		public WallAnimationComponent(string animationName, string layer = null, string tag = null) : base(animationName, layer, tag) {
		}

		public WallAnimationComponent(string animationName, ColorSet set) : base(animationName, set) {
		}

		protected override void CallRender(Vector2 pos) {
			var region = Animation.GetCurrentTexture();
			var origin = new Vector2(region.Width / 2, region.Height);
			
			Graphics.Render(region, pos + origin, Angle, origin);
		}
	}
}