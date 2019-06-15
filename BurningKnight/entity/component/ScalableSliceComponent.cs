using Lens.graphics;
using Lens.graphics.animation;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class ScalableSliceComponent : SliceComponent {
		public Vector2 Scale = Vector2.One;
		
		public ScalableSliceComponent(string image, string slice) : base(image, slice) {
			
		}

		public ScalableSliceComponent(AnimationData image, string slice) : base(image, slice) {
			
		}

		public override void Render(bool shadow) {
			var origin = Sprite.Center;
			
			if (shadow) {
				Graphics.Render(Sprite, Entity.Position + origin + new Vector2(0, ShadowZ + Sprite.Height), 0, origin, Scale, Graphics.ParseEffect(Flipped, !FlippedVerticaly));
				return;
			}
			
			Graphics.Render(Sprite, Entity.Position + origin, 0, origin, Scale);
		}
	}
}