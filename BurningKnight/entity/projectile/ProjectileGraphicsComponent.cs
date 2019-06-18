using BurningKnight.assets;
using BurningKnight.entity.component;
using Lens.graphics;
using Lens.graphics.animation;
using Microsoft.Xna.Framework;
using VelcroPhysics.Utilities;
using MathUtils = Lens.util.MathUtils;

namespace BurningKnight.entity.projectile {
	public class ProjectileGraphicsComponent : SliceComponent {
		public ProjectileGraphicsComponent(string image, string slice) : base(image, slice) {
			
		}

		public ProjectileGraphicsComponent(AnimationData image, string slice) : base(image, slice) {
			
		}

		public override void Render(bool shadow) {
			var p = (Projectile) Entity;
			var scale = new Vector2(p.Scale);
			
			if (shadow) {
				Graphics.Render(Sprite, Entity.Position + new Vector2(Sprite.Center.X, Sprite.Height + Sprite.Center.Y + 4), 
					0, Sprite.Center, scale, Graphics.ParseEffect(Flipped, !FlippedVerticaly));
				return;
			}

			var d = p.IndicateDeath && p.T >= p.Range - 1.8f && p.T % 0.6f >= 0.3f;

			if (d) {
				var shader = Shaders.Entity;
				
				Shaders.Begin(shader);
				
				shader.Parameters["flash"].SetValue(1f);
				shader.Parameters["flashReplace"].SetValue(1f);
				shader.Parameters["flashColor"].SetValue(ColorUtils.White);
			}
			
			Graphics.Render(Sprite, Entity.Position + Sprite.Center, 0, Sprite.Center, scale, Graphics.ParseEffect(Flipped, FlippedVerticaly));

			if (d) {
				Shaders.End();
			}
		}
	}
}