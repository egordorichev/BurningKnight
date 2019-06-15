using BurningKnight.assets;
using Lens.assets;
using Lens.entity.component.graphics;
using Lens.graphics;
using Lens.graphics.animation;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class SliceComponent : GraphicsComponent {
		public TextureRegion Sprite;
		public int ShadowZ;
		
		public SliceComponent(string image, string slice) {
			Sprite = Animations.Get(image).GetSlice(slice);
		}

		public SliceComponent(AnimationData image, string slice) {
			Sprite = image.GetSlice(slice);
		}

		public void AddShadow() {
			Entity.AddComponent(new ShadowComponent(RenderShadow));
		}

		private void RenderShadow() {
			Render(true);
		}

		public void SetOwnerSize() {
			Entity.Width = Sprite.Width;
			Entity.Height = Sprite.Height;
		}

		public override void Render(bool shadow) {
			if (shadow) {
				Graphics.Render(Sprite, Entity.Position + new Vector2(0, Sprite.Height + ShadowZ), 0, Vector2.Zero, Vector2.One, Graphics.ParseEffect(Flipped, !FlippedVerticaly));
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
			
			Graphics.Render(Sprite, Entity.Position);
			
			if (stopShader) {
				Shaders.End();
			}
		}
	}
}