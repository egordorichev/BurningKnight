using System;
using BurningKnight.assets.lighting;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using Lens.entity;
using Lens.graphics;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.item.util {
	public class MeleeArc : Entity {
		public static Color ReflectedColor = new Color(0.5f, 1f, 0.5f, 1f);
		
		public float LifeTime = 0.1f;
		public int Damage;
		public Entity Owner;
		public float Angle;

		private float t;
		private Vector2 velocity;

		public override void AddComponents() {
			base.AddComponents();

			float force = 40f;
			velocity = new Vector2((float) Math.Cos(Angle) * force, (float) Math.Sin(Angle) * force);
			
			AddComponent(new RectBodyComponent(0, -Height / 2f, Width, Height, BodyType.Dynamic, true) {
				Angle = Angle
			});

			AddComponent(new AnimationComponent("sword_trail", null, "idle") {
				Offset = new Vector2(4, 12),
				Scale = new Vector2(Width / 8, Height / 24)
			});

			GetComponent<AnimationComponent>().OriginY = 12;
		}

		public override void Render() {
			var component = GetComponent<AnimationComponent>();
			var region = component.Animation.GetCurrentTexture();

			Graphics.Render(region, Position, Angle, component.Offset, component.Scale);
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent ev) {
				/*if (ev.Entity is DestroyableLevel) {
					var fixture = Physics.Fixture;
					fixture.GetAABB(out var hitbox, 0);

					Run.Level.Destroyable.Break(hitbox.Center.X, hitbox.Center.Y);
				} else */

				if (ev.Entity is Bomb) {
					ev.Entity.GetComponent<RectBodyComponent>().KnockbackFrom(Owner);
				} else if (ev.Entity is Projectile p) {
					if (p.CanBeReflected) {
						p.Owner = this;

						var b = p.BodyComponent;
						var d = Math.Max(200, b.Velocity.Length() * 1.2f);
						var a = Owner.AngleTo(p);

						b.Velocity = new Vector2((float) Math.Cos(a) * d, (float) Math.Sin(a) * d);

						if (p.TryGetComponent<LightComponent>(out var l)) {
							l.Light.Color = ReflectedColor;
						}
						
						Camera.Instance.ShakeMax(4f);
					} else if (p.CanBeBroken) {
						p.Break();
					}
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