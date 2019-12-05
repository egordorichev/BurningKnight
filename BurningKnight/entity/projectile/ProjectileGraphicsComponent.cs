using BurningKnight.assets;
using BurningKnight.assets.lighting;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using Lens.assets;
using Lens.graphics;
using Lens.graphics.animation;
using Microsoft.Xna.Framework;
using VelcroPhysics.Utilities;
using MathUtils = Lens.util.MathUtils;

namespace BurningKnight.entity.projectile {
	public class ProjectileGraphicsComponent : SliceComponent {
		public static TextureRegion Flash;
		public bool IgnoreRotation;
		public float Rotation => IgnoreRotation ? 0 : ((Projectile) Entity).BodyComponent.Body.Rotation;
		public TextureRegion Aura;
		public TextureRegion Light;
		
		public ProjectileGraphicsComponent(string image, string slice) : base(image, slice) {
			if (Flash == null) {
				Flash = CommonAse.Particles.GetSlice("flash");
			}

			var a = Animations.Get(image);
			
			Aura = a.GetSlice($"{slice}_aura", false);
			Light = a.GetSlice($"{slice}_light", false);
		}

		public override void Init() {
			base.Init();

			if (Light == null) {
				((Projectile) Entity).Color = ColorUtils.WhiteColor;
			}
		}

		public override void Render(bool shadow) {
			var p = (Projectile) Entity;
			var scale = new Vector2(p.Scale);
			var a = Rotation;
			var b = p.FlashTimer > 0;
			var spr = b ? Flash : Sprite;
			var or = spr.Center;
			
			if (shadow) {
				Graphics.Render(spr, Entity.Center + new Vector2(0, 6), 
					a, or, scale);
				return;
			}

			var d = p.Dying || (p.IndicateDeath && p.NearingDeath);
			var started = false;
			
			if (!d) {
				// fixme: p.Effect.GetColor()
				Graphics.Color = p.Color;
			} else if (Light == null) {
				var shader = Shaders.Entity;
				Shaders.Begin(shader);

				shader.Parameters["flash"].SetValue(1f);
				shader.Parameters["flashReplace"].SetValue(1f);
				shader.Parameters["flashColor"].SetValue(ColorUtils.White);
				
				started = true;
			}
			
			Graphics.Render(spr, Entity.Center, a, or, scale);
			Graphics.Color = ColorUtils.WhiteColor;

			if (started) {
				Shaders.End();
			}
			
			if (!b && Light != null) {
				Graphics.Render(Light, Entity.Center, a, or, scale);
			}
		}
		
		public void RenderLight() {
			if (Aura != null) {
				var p = (Projectile) Entity;

				if (!(p.Dying || (p.IndicateDeath && p.NearingDeath))) {
					Graphics.Color = p.Color;
				}
				
				Graphics.Color.A = Lights.AuraAlpha;
				Graphics.Render(Aura, Entity.Center, Rotation, Aura.Center, new Vector2(((Projectile) Entity).Scale));
				Graphics.Color.A = 255;
				Graphics.Color = ColorUtils.WhiteColor;
			}
		}
	}
}