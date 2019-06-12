using System;
using BurningKnight.assets.lighting;
using BurningKnight.assets.particle;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.level;
using BurningKnight.level.entities;
using BurningKnight.physics;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.graphics;
using Lens.util;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.projectile {
	public class Projectile : Entity, CollisionFilterEntity {
		public BodyComponent BodyComponent;
		public int Damage = 1;
		public Entity Owner;
		public float Range = -1;
		public float T;
		public int BounceLeft = -1;
		public bool IndicateDeath;
		public bool CanBeReflected = true;
		public bool CanBeBroken = true;
		public Action<Projectile, bool> OnDeath;
		public Action<Projectile, float> Controller;
		
		internal Projectile() {}

		public static Projectile Make(Entity owner, string slice, double angle, float speed, bool circle = true, int bounce = -1) {
			var projectile = new Projectile();
			owner.Area.Add(projectile);
			
			projectile.Owner = owner;
			projectile.BounceLeft = bounce;

			var graphics = new ProjectileGraphicsComponent("projectiles", slice);
			projectile.AddComponent(graphics);

			var w = graphics.Sprite.Source.Width;
			var h = graphics.Sprite.Source.Height;

			projectile.Width = w;
			projectile.Height = h;
			projectile.Center = owner.Center;

			if (circle) {
				projectile.AddComponent(projectile.BodyComponent = new CircleBodyComponent(0, 0, w / 2f));
			} else {
				projectile.AddComponent(projectile.BodyComponent = new RectBodyComponent(0, 0, w, h));
			}

			speed *= 10f;
			
			projectile.BodyComponent.Velocity = new Vector2((float) (Math.Cos(angle) * speed), (float) (Math.Sin(angle) * speed));

			projectile.BodyComponent.Body.Restitution = 1;
			projectile.BodyComponent.Body.Friction = 0;
			projectile.BodyComponent.Body.IsBullet = true;
      			
			return projectile;
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new ShadowComponent(RenderShadow));
			AlwaysActive = true;
		}

		private void RenderShadow() {
			GraphicsComponent.Render(true);
		}

		public void AddLight(float radius, Color color) {
			AddComponent(new LightComponent(this, radius, color));
		}

		public override void Update(float dt) {
			base.Update(dt);

			T += dt;

			if (Range > -1 && T >= Range) {
				AnimateDeath(true);
				return;
			}
			
			Controller?.Invoke(this, dt);
		}

		protected bool BreaksFrom(Entity entity) {
			return entity != Owner && (!(entity is Creature) || Owner is Mob != entity is Mob) && 
			       (entity is Level || (entity is Door d && !d.Open) || entity.HasComponent<HealthComponent>() || 
			        entity is SolidProp || entity is DestroyableLevel || entity is ItemStand);
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent ev) {
				if (ev.Entity != Owner && (!(Owner is Creature ac) || !(ev.Entity is Creature bc) || ac.IsFriendly() != bc.IsFriendly()) && ev.Entity.TryGetComponent<HealthComponent>(out var health)) {
					health.ModifyHealth(-Damage, Owner);
				}
				
				if (BreaksFrom(ev.Entity)) {
					if (BounceLeft > 0) {
						BounceLeft -= 1;
					} else {
						AnimateDeath();
					}
					
					if (ev.Entity is DestroyableLevel lvl) {
						lvl.Break(CenterX, CenterY);
					}
				}
			}
			
			return base.HandleEvent(e);
		}

		public bool ShouldCollide(Entity entity) {
			return !((entity is Creature && Owner is Mob == entity is Mob) || entity is Creature || entity is Chasm || entity is Item || entity is Projectile || entity is Prop);
		}

		public void Break() {
			AnimateDeath();
		}
		
		protected virtual void AnimateDeath(bool timeout = false) {
			if (Done) {
				return;
			}
			
			Done = true;
			
			var explosion = new ParticleEntity(Particles.Animated("bullet_fx"));
			explosion.Position = Center;
			Area.Add(explosion);
			explosion.Depth = 30;
			explosion.Particle.AngleVelocity = 0;
			explosion.Particle.Velocity = Vector2.Zero;
			explosion.AddShadow();
			
			OnDeath?.Invoke(this, timeout);
		}
	}
}