using Lens.graphics;
using Lens.graphics.animation;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class ZAnimationComponent : AnimationComponent {
		public Vector2 Scale = Vector2.One;
		
		public ZAnimationComponent(string animationName, string layer = null, string tag = null) : base(animationName, layer, tag) {
		}

		public ZAnimationComponent(string animationName, ColorSet set) : base(animationName, set) {
		}

		protected override void CallRender(Vector2 pos, bool shadow) {
			var component = GetComponent<ZComponent>();
			
			var region = Animation.GetCurrentTexture();
			var origin = new Vector2(region.Source.Width / 2f, FlippedVerticaly ? 0 : region.Source.Height);

			if (!shadow) {
				pos.Y -= component.Z;
			}
			
			Graphics.Render(region, pos + origin, shadow ^ Flipped ? -Angle : Angle, origin, Scale, Graphics.ParseEffect(Flipped, FlippedVerticaly));
		}
	}
}