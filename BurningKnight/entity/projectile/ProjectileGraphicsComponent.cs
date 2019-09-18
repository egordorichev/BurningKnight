using BurningKnight.assets;
using BurningKnight.entity.component;
using Lens.graphics;
using Lens.graphics.animation;
using Microsoft.Xna.Framework;
using VelcroPhysics.Utilities;
using MathUtils = Lens.util.MathUtils;

namespace BurningKnight.entity.projectile {
	public class ProjectileGraphicsComponent : SliceComponent {
		public static TextureRegion Flash;
		
		public ProjectileGraphicsComponent(string image, string slice) : base(image, slice) {
			if (Flash == null) {
				Flash = CommonAse.Particles.GetSlice("flash");
			}
		}

		public override void Render(bool shadow) {
			var p = (Projectile) Entity;
			var scale = new Vector2(p.Scale);
			var a = p.BodyComponent.Body.Rotation;
			
			var spr = p.FlashTimer > 0 ? Flash : Sprite;
			var or = spr.Center; // new Vector2(p.Width / 2, p.Height / 2);
			
			if (shadow) {
				Graphics.Render(spr, Entity.Center + new Vector2(0, 8), 
					a, or, scale);
				return;
			}

			var d = p.Dying || (p.IndicateDeath && p.T >= p.Range - 1.8f && p.T % 0.6f >= 0.3f);

			if (d) {
				var shader = Shaders.Entity;
				
				Shaders.Begin(shader);
				
				shader.Parameters["flash"].SetValue(1f);
				shader.Parameters["flashReplace"].SetValue(1f);
				shader.Parameters["flashColor"].SetValue(ColorUtils.White);
			}
			
			Graphics.Render(spr, Entity.Center, a, or, scale);

			if (d) {
				Shaders.End();
			}
		}
	}
}