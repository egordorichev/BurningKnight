using System;
using BurningKnight.assets;
using Lens.graphics;
using Lens.graphics.animation;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class ScalableSliceComponent : SliceComponent {
		public Vector2 Scale = Vector2.One;
		public Vector2 Origin;
		
		public ScalableSliceComponent(TextureRegion region) : base("", null) {
			Sprite = region;
		}
		
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
			
			var stopShader = false;

			if (Entity.TryGetComponent<HealthComponent>(out var health) && health.RenderInvt) {
				var i = health.InvincibilityTimer;

				if (i > health.InvincibilityTimerMax / 2f || i % 0.1f > 0.05f) {
					var shader = Shaders.Entity;
					Shaders.Begin(shader);

					shader.Parameters["flash"].SetValue(1f);
					shader.Parameters["flashReplace"].SetValue(1f);
					shader.Parameters["flashColor"].SetValue(ColorUtils.White);
					
					stopShader = true;
				}
			}
			
			Graphics.Render(Sprite, Entity.Position + Origin, Angle, Origin, Scale);
			
			if (stopShader) {
				Shaders.End();
			}
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