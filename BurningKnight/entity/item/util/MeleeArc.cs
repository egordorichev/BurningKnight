using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.level;
using BurningKnight.physics;
using BurningKnight.state;
using Lens.entity;
using Lens.graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Shared;

namespace BurningKnight.entity.item.util {
	public class MeleeArc : Entity {
		public float LifeTime = 0.1f;
		public int Damage;
		public Entity Owner;
		public float Angle;

		private float t;
		private Vector2 velocity;

		public override void AddComponents() {
			base.AddComponents();

			Width = 8;
			Height = 24;

			float force = 40f;
			velocity = new Vector2((float) Math.Cos(Angle) * force, (float) Math.Sin(Angle) * force);
			
			AddComponent(new RectBodyComponent(0, -Height / 2f, Width, Height, BodyType.Dynamic, true) {
				Angle = Angle
			});
			
			AddComponent(new AnimationComponent("sword_trail", null, "idle") {
				Offset = new Vector2(4, 12)
			});
		}

		public override void Render() {
			var component = GetComponent<AnimationComponent>();
			var region = component.Animation.GetCurrentTexture();

			Graphics.Render(region, Position, Angle, component.Offset, Vector2.One);
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent ev) {
				if (ev.Entity is DestroyableLevel) {
					var fixture = Physics.Fixture;
					fixture.GetAABB(out var hitbox, 0);

					Run.Level.Destroyable.Break(hitbox.Center.X, hitbox.Center.Y);

				} else if (ev.Entity != Owner && ev.Entity.TryGetComponent<HealthComponent>(out var health)) {
					health.ModifyHealth(-Damage, Owner);
				}
			}
			
			return base.HandleEvent(e);
		}

		public override void Update(float dt) {
			base.Update(dt);

			t += dt;
			Position = Owner.Center + velocity * t;

			if (t >= LifeTime) {
				Done = true;
			}
		}
	}
}