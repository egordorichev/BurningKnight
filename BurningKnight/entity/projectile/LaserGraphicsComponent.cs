using System;
using BurningKnight.assets;
using BurningKnight.assets.lighting;
using BurningKnight.entity.component;
using Lens.assets;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.projectile {
	public class LaserGraphicsComponent : BasicProjectileGraphicsComponent {
		private TextureRegion aura;
		private TextureRegion light;
		private TextureRegion end;
		private TextureRegion endAura;
		private TextureRegion endLight;
		
		private Vector2 origin;
		private Vector2 centerOrigin;
		private Vector2 lightOrigin;
		public float Rotation => ((Projectile) Entity).GetAnyComponent<BodyComponent>().Body.Rotation;
		
		public LaserGraphicsComponent(string image, string slice) : base(image, slice) {
			var a = Animations.Get(image);
			
			aura = a.GetSlice($"{slice}_aura", false);
			light = a.GetSlice($"{slice}_light", false);
			end = a.GetSlice("end_aura", false);
			endAura = a.GetSlice("end", false);
			endLight = a.GetSlice("end_light", false);

			origin = new Vector2(0, aura.Height * 0.5f);
			centerOrigin = new Vector2(0, Sprite.Height * 0.5f);
			lightOrigin = new Vector2(0, light.Height * 0.5f);
		}

		public override void Render(bool shadow) {
			var scale = new Vector2(Entity.Width / aura.Width, Entity.Height / aura.Height);
			var p = (Laser) Entity;
			var a = Rotation;

			Graphics.Color = p.Color;
			Graphics.Color.A = (byte) (Math.Min(1f, ((Laser) Entity).LifeTime * 3f) * 255);

			if (shadow) {
				Graphics.Render(Sprite, Entity.Position + new Vector2(0, 6), a, centerOrigin, scale);
			} else {
				Graphics.Render(Sprite, Entity.Position, a, centerOrigin, scale);
				
				var sc = new Vector2(p.Scale);
				var aa = a + (float) Math.PI * 0.25f;

				Graphics.Render(endAura, Entity.Position, aa, endAura.Center, sc);
				Graphics.Render(endAura, p.End, aa, endAura.Center, sc);
			}

			Graphics.Color.A = 255;
			Graphics.Color = ColorUtils.WhiteColor;
		}
		
		public void RenderTopLight() {
			var p = (Laser) Entity;
			var s = p.Scale;
			var a = Rotation;

			Graphics.Color.A = (byte) (Math.Min(1f, ((Laser) Entity).LifeTime * 3f) * 255);

			var sc = new Vector2(p.Scale);
			var aa = a + (float) Math.PI * 0.25f;

			Graphics.Render(endLight, Entity.Position, aa, endLight.Center, sc);
			Graphics.Render(endLight, p.End, aa, endLight.Center, sc);
			
			Graphics.Render(light, Entity.Position, a, lightOrigin, new Vector2(Entity.Width, s));

			Graphics.Color.A = 255;
		}

		public override void RenderLight() {
			var p = (Laser) Entity;
			var a = Rotation;
			var sc = new Vector2(p.Scale);

			Graphics.Color = p.Color;
			Graphics.Color.A = (byte) (Math.Min(1f, ((Laser) Entity).LifeTime * 3f) * Lights.AuraAlpha);
			
			var aa = a + (float) Math.PI * 0.25f;
			Graphics.Render(end, Entity.Position, aa, end.Center, sc);
			Graphics.Render(end, p.End, aa, end.Center, sc);
			
			Graphics.Render(aura, Entity.Position, a, origin, new Vector2(Entity.Width / aura.Width, Entity.Height / aura.Height));
			Graphics.Color.A = 255;
			Graphics.Color = ColorUtils.WhiteColor;
		}
	}
}