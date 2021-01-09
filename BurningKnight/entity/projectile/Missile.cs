using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.level;
using BurningKnight.level.entities;
using BurningKnight.physics;
using BurningKnight.util;
using Lens.entity;
using Lens.graphics;
using Lens.util.camera;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;

namespace BurningKnight.entity.projectile {
	/*public class Missile : Projectile {
		private const float MinUpTime = 2f;
		
		private Entity target;
		private bool goingDown;
		private float toY;
		private float shadowSize;
		private bool exploded;
		public bool HurtOwner = true;
		
		public Missile(Entity owner, Entity tar) {
			target = tar;
			Owner = owner;
		}

		public override void AddComponents() {
			Slice = "missile";
			base.AddComponents();

			AlwaysVisible = true;
			AlwaysActive = true;
			
			var graphics = new ProjectileGraphicsComponent("projectiles", Slice);
			AddComponent(graphics);

			var w = graphics.Sprite.Source.Width;
			var h = graphics.Sprite.Source.Height;

			Width = w;
			Height = h;
			Center = Owner.Center;
			
			AddComponent(BodyComponent = new RectBodyComponent(0, 0, w, h));
			
			BodyComponent.Body.IsBullet = true;
			BodyComponent.Body.LinearVelocity = new Vector2(0, -100f);

			Owner.HandleEvent(new ProjectileCreatedEvent {
				Owner = Owner,
				Projectile = this
			});

			Depth = Layers.TileLights;
			
			CollisionFilterComponent.Add(this, (p, e) => {
				if (e is Level || e is Prop) {
					return CollisionResult.Disable;
				}
				
				return CollisionResult.Default;
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
			Position += BodyComponent.Velocity * dt;

			if (goingDown) {
				if (Bottom >= toY && !exploded) {
					AnimateDeath(null);
				}
			} else if (T >= MinUpTime && Bottom < Camera.Instance.Y) {
				goingDown = true;
				CenterX = target.CenterX;
				toY = target.Bottom;
				GraphicsComponent.FlippedVerticaly = true;
				BodyComponent.Body.LinearVelocity = new Vector2(0, 100f);

				Tween.To(16, 0, x => shadowSize = x, 1f);
			}
		}

		protected override void AnimateDeath(Entity e, bool timeout = false) {
			base.AnimateDeath(e, timeout);
			
			ExplosionMaker.Make(this, damageOwner: HurtOwner);
			exploded = true;
		}

		protected override void RenderShadow() {
			if (goingDown) {
				Graphics.Batch.DrawCircle(CenterX, toY, shadowSize, 16, ColorUtils.WhiteColor, 2f);
				Graphics.Batch.DrawCircle(CenterX, toY, shadowSize * 0.5f, 16, ColorUtils.WhiteColor, 2f);
			}
		}

		public override bool BreaksFrom(Entity entity, BodyComponent body) {
			return false;
		}

		public override bool ShouldCollide(Entity entity) {
			return false;
		}
	}*/
}