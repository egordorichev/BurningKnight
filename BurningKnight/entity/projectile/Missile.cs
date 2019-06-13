using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.util;
using Lens.entity;
using Lens.graphics;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;

namespace BurningKnight.entity.projectile {
	public class Missile : Projectile {
		private const float MinUpTime = 2f;
		
		private Entity target;
		private bool goingDown;
		private float toY;
		
		public Missile(Entity owner, Entity tar) {
			owner.Area.Add(this);
			
			Slice = "missile";
			Owner = owner;
			target = tar;
			
			var graphics = new ProjectileGraphicsComponent("projectiles", Slice);
			AddComponent(graphics);

			var w = graphics.Sprite.Source.Width;
			var h = graphics.Sprite.Source.Height;

			Width = w;
			Height = h;
			Center = owner.Center;
			
			AddComponent(BodyComponent = new RectBodyComponent(0, 0, w, h));
			
			BodyComponent.Body.IsBullet = true;
			BodyComponent.Body.LinearVelocity = new Vector2(0, -100f);

			owner.HandleEvent(new ProjectileCreatedEvent {
				Owner = owner,
				Projectile = this
			});
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent) {
				return false; // Ignore all collision
			}
			
			return base.HandleEvent(e);
		}

		public override void Update(float dt) {
			base.Update(dt);

			// fixme: need a shadow or rings like before
			Position += BodyComponent.Velocity * dt;

			if (goingDown) {
				if (Bottom >= toY) {
					AnimateDeath();
				}
			} else if (T >= MinUpTime && Bottom < Camera.Instance.Y) {
				goingDown = true;
				CenterX = target.CenterX;
				toY = target.Bottom;
				GraphicsComponent.FlippedVerticaly = true;
				BodyComponent.Body.LinearVelocity = new Vector2(0, 100f);
			}
		}

		protected override void AnimateDeath(bool timeout = false) {
			base.AnimateDeath(timeout);
			AnimationUtil.Explosion(Center);
		}

		protected override void RenderShadow() {
			
		}
	}
}