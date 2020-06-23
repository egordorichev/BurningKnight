using BurningKnight.assets;
using BurningKnight.assets.items;
using Lens.assets;
using Lens.entity.component.graphics;
using Lens.graphics;
using Lens.graphics.animation;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class SliceComponent : GraphicsComponent {
		public TextureRegion Sprite;
		public int ShadowZ;
		public float Angle;
		
		public SliceComponent(string image, string slice) {
			Set(image, slice);
		}

		public SliceComponent(AnimationData image, string slice) {
			Set(image.GetSlice(slice));
		}

		public SliceComponent(TextureRegion region) {
			Set(region);
		}

		public void Set(string image, string slice) {
			if (image == null || slice == null) {
				Sprite = Textures.Missing;
				return;
			}
			
			Set(Animations.Get(image).GetSlice(slice));
		}
		
		public virtual void Set(TextureRegion region) {
			Sprite = region;
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
			var o = Sprite.Center;
			
			if (shadow) {
				Graphics.Render(Sprite, Entity.Position + new Vector2(0, (int) Sprite.Height + ShadowZ) + o, Angle, o, Vector2.One, Graphics.ParseEffect(Flipped, ShadowZ > 0 ? FlippedVerticaly : !FlippedVerticaly));
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
			
			Graphics.Render(Sprite, Entity.Position + o, Angle, o);
			
			if (stopShader) {
				Shaders.End();
			}
		}
	}
}