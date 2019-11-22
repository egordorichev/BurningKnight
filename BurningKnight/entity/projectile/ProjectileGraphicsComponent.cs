using BurningKnight.assets;
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
		public TextureRegion Big;
		
		public ProjectileGraphicsComponent(string image, string slice) : base(image, slice) {
			if (Flash == null) {
				Flash = CommonAse.Particles.GetSlice("flash");
			}

			Big = Animations.Get(image).GetSlice($"{slice}_big");
		}

		public override void Render(bool shadow) {
			var p = (Projectile) Entity;
			var scale = new Vector2(p.Scale);
			var a = Rotation;
			
			var spr = p.FlashTimer > 0 ? Flash : Sprite;
			var or = spr.Center; // new Vector2(p.Width / 2, p.Height / 2);
			
			if (shadow) {
				Graphics.Render(spr, Entity.Center + new Vector2(0, 6), 
					a, or, scale);
				return;
			}

			var d = p.Dying || (p.IndicateDeath && p.T >= p.Range - 1.8f && p.T % 0.6f >= 0.3f);
			var started = d;

			if (d) {
				var shader = Shaders.Entity;
				Shaders.Begin(shader);
				
				shader.Parameters["flash"].SetValue(1f);
				shader.Parameters["flashReplace"].SetValue(1f);
				shader.Parameters["flashColor"].SetValue(ColorUtils.White);
			} else if (p.Effect != ProjectileGraphicsEffect.Normal) {
				started = true;
				
				var shader = Shaders.Entity;
				Shaders.Begin(shader);
				
				shader.Parameters["flash"].SetValue(1f);
				shader.Parameters["flashReplace"].SetValue(0f);
				shader.Parameters["flashColor"].SetValue(p.Effect.GetColor());
			}
			
			Graphics.Render(spr, Entity.Center, a, or, scale);

			if (started) {
				Shaders.End();
			}
		}
		
		public void RenderLight() {
			if (Big != null) {
				Graphics.Render(Big, Entity.Center, Rotation, Big.Center, new Vector2(((Projectile) Entity).Scale));
			}
		}
	}
}