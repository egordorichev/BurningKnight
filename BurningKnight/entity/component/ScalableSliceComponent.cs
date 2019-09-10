using Lens.graphics;
using Lens.graphics.animation;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class ScalableSliceComponent : SliceComponent {
		public Vector2 Scale = Vector2.One;
		public Vector2 Origin;
		
		public ScalableSliceComponent(string image, string slice) : base(image, slice) {
			
		}

		public ScalableSliceComponent(AnimationData image, string slice) : base(image, slice) {
			
		}

		public override void Set(TextureRegion region) {
			base.Set(region);
			Origin = Sprite.Center;
		}

		public override void Render(bool shadow) {
			if (shadow) {
				Graphics.Render(Sprite, Entity.Position + Origin + new Vector2(0, ShadowZ + Sprite.Height), Angle, Origin, Scale, Graphics.ParseEffect(Flipped, ShadowZ > 0 ? FlippedVerticaly : !FlippedVerticaly));
				return;
			}
			
			Graphics.Render(Sprite, Entity.Position + Origin, Angle, Origin, Scale);
		}
	}
}