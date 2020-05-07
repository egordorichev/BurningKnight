using System;
using BurningKnight.assets;
using BurningKnight.assets.lighting;
using Lens.assets;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.projectile {
	public class LaserGraphicsComponent : BasicProjectileGraphicsComponent {
		private TextureRegion aura;
		private Vector2 origin;
		private Vector2 centerOrigin;
		public float Rotation => ((Projectile) Entity).BodyComponent.Body.Rotation;
		
		public LaserGraphicsComponent(string image, string slice) : base(image, slice) {
			aura = Animations.Get(image).GetSlice($"{slice}_aura", false);
			origin = new Vector2(0, aura.Height * 0.5f);
			centerOrigin = new Vector2(0, Sprite.Height * 0.5f);
		}

		public override void Render(bool shadow) {
			var scale = new Vector2(Entity.Width / aura.Width, Entity.Height / aura.Height);
			var a = Rotation;

			if (shadow) {
				Graphics.Render(Sprite, Entity.Position + new Vector2(0, 6), a, centerOrigin, scale);
				return;
			}

			Graphics.Color.A = (byte) (Math.Min(1f, ((Laser) Entity).LifeTime * 3f) * 255);
			Graphics.Render(Sprite, Entity.Position, a, centerOrigin, scale);
			Graphics.Color.A = 255;
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

				Graphics.Color.A = (byte) (Math.Min(1f, ((Laser) Entity).LifeTime * 3f) * Lights.AuraAlpha);
				Graphics.Render(aura, Entity.Position, Rotation, origin, new Vector2(Entity.Width / aura.Width, Entity.Height / aura.Height));
				Graphics.Color.A = 255;
				Graphics.Color = ColorUtils.WhiteColor;
			}
		}
	}
}