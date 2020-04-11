using System;
using Lens.assets;
using Lens.entity.component.graphics;
using Lens.graphics;
using Lens.graphics.animation;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class ZSliceComponent : GraphicsComponent {
		public TextureRegion Sprite;
		public Vector2 Scale = Vector2.One;
		public Vector2 Origin;
		
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
				Graphics.Render(Sprite, Entity.Position + new Vector2(0, Sprite.Height), 0, Origin, Scale, Graphics.ParseEffect(Flipped, !FlippedVerticaly));
				return;
			}
			
			Graphics.Render(Sprite, Entity.Position - new Vector2(0, Entity.GetComponent<ZComponent>().Z), 0, Origin, Scale, Graphics.ParseEffect(Flipped, FlippedVerticaly));
		}
		
		public void Animate(Action callback = null) {
			Tween.To(1.8f, Scale.X, x => Scale.X = x, 0.1f);
			Tween.To(0.2f, Scale.Y, x => Scale.Y = x, 0.1f).OnEnd = () => {
				Tween.To(1, Scale.X, x => Scale.X = x, 0.4f);
				Tween.To(1, Scale.Y, x => Scale.Y = x, 0.4f);

				callback?.Invoke();
			};
		}
	}
}