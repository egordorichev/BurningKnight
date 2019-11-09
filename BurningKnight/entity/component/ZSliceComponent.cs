using Lens.assets;
using Lens.entity.component.graphics;
using Lens.graphics;
using Lens.graphics.animation;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class ZSliceComponent : GraphicsComponent {
		public TextureRegion Sprite;
		public Vector2 Scale = Vector2.One;
		
		public ZSliceComponent(TextureRegion region) {
			Sprite = region;
		}
		
		public ZSliceComponent(string image, string slice) {
			Sprite = Animations.Get(image).GetSlice(slice);
		}

		public ZSliceComponent(AnimationData image, string slice) {
			Sprite = image.GetSlice(slice);
		}

		public override void Render(bool shadow) {
			if (shadow) {
				Graphics.Render(Sprite, Entity.Position + new Vector2(0, Sprite.Height), 0, Vector2.Zero, Scale, Graphics.ParseEffect(Flipped, !FlippedVerticaly));
				return;
			}
			
			Graphics.Render(Sprite, Entity.Position - new Vector2(0, Entity.GetComponent<ZComponent>().Z), 0, Vector2.Zero, Scale, Graphics.ParseEffect(Flipped, FlippedVerticaly));
		}
	}
}