using BurningKnight.assets;
using BurningKnight.assets.lighting;
using Lens.assets;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.projectile {
	public class LaserGraphicsComponent : BasicProjectileGraphicsComponent {
		private TextureRegion aura;
		public float Rotation => ((Projectile) Entity).BodyComponent.Body.Rotation;
		
		public LaserGraphicsComponent(string image, string slice) : base(image, slice) {
			aura = Animations.Get(image).GetSlice($"{slice}_aura", false);
		}

		public override void Render(bool shadow) {
			var scale = new Vector2(Entity.Width / aura.Width, Entity.Height / aura.Height);
			var a = Rotation;
			var or = Sprite.Center;

			if (shadow) {
				Graphics.Render(Sprite, Entity.Center + new Vector2(0, 6), a, or, scale);

				return;
			}

			Graphics.Render(Sprite, Entity.Center, a, or, scale);
			Graphics.Color = ColorUtils.WhiteColor;
		}

		public override void RenderLight() {
			if (aura != null) {
				var p = (Projectile) Entity;

				if (p.Scourged) {
					return;
				}

				if (!(p.Dying || (p.IndicateDeath && p.NearingDeath))) {
					Graphics.Color = p.Color;
				}

				Graphics.Color.A = Lights.AuraAlpha;
				Graphics.Render(aura, Entity.Center, Rotation, aura.Center, new Vector2(Entity.Width / aura.Width, Entity.Height / aura.Height));
				Graphics.Color.A = 255;
				Graphics.Color = ColorUtils.WhiteColor;
			}
		}
	}
}