using Lens.assets;
using Lens.graphics;
using Lens.graphics.animation;
using Lens.util;
using Microsoft.Xna.Framework;

namespace Lens.entity.component.graphics {
	public class SliceComponent : GraphicsComponent {
		public TextureRegion Sprite;
		
		public SliceComponent(string image, string slice) {
			Sprite = Animations.Get(image).GetSlice(slice);
			Check(slice);
		}

		public SliceComponent(AnimationData image, string slice) {
			Sprite = image.GetSlice(slice);
			Check(slice);
		}

		private void Check(string slice) {
			if (Sprite == null) {
				Log.Warning($"Unable to find slice {slice}");
			}
		}

		public override void Render(bool shadow) {
			if (shadow) {
				Graphics.Render(Sprite, Entity.Position + new Vector2(0, Sprite.Height), 0, Vector2.Zero, Vector2.One, Graphics.ParseEffect(Flipped, !FlippedVerticaly));
				return;
			}
			
			Graphics.Render(Sprite, Entity.Position);
		}
	}
}