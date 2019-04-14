using Lens.entity.component.graphics;
using Lens.graphics;
using Lens.graphics.animation;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.projectile {
	public class ProjectileGraphicsComponent : SliceComponent {
		public ProjectileGraphicsComponent(string image, string slice) : base(image, slice) {
		}

		public ProjectileGraphicsComponent(AnimationData image, string slice) : base(image, slice) {
		}

		public override void Render(bool shadow) {
			if (shadow) {
				Graphics.Render(Sprite, Entity.Position + new Vector2(Sprite.Center.X, Sprite.Height + Sprite.Center.Y), 
					0, Sprite.Center, new Vector2(0.5f), Graphics.ParseEffect(Flipped, !FlippedVerticaly));
				return;
			}
			
			Graphics.Render(Sprite, Entity.Position);
		}
	}
}