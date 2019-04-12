using System;
using BurningKnight.assets.lighting;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.level;
using BurningKnight.level.entities;
using BurningKnight.physics;
using Lens.entity;
using Lens.entity.component.graphics;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.projectile {
	public class Projectile : Entity, CollisionFilterEntity {
		protected BodyComponent BodyComponent;
		public int Damage = 1;
		public Entity Owner;
		public float Range = -1;
		public float T;
		
		internal Projectile() {}

		public static Projectile Make(Entity owner, string slice, double angle, float speed, bool circle = true) {
			var projectile = new Projectile();
			owner.Area.Add(projectile);
			
			projectile.Owner = owner;
			
			var graphics = new SliceComponent("projectiles", slice);
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
			
			projectile.BodyComponent.Velocity = new Vector2((float) (Math.Cos(angle) * speed), (float) (Math.Sin(angle) * speed));

			return projectile;
		}

		public void AddLight(float radius, Color color) {
			AddComponent(new LightComponent(this, radius, color));
		}

		public override void Update(float dt) {
			base.Update(dt);

			T += dt;

			if (Range > -1 && T >= Range) {
				Done = true;
				return;
			}
			
			Position += BodyComponent.Velocity * dt;
		}

		protected bool BreaksFrom(Entity entity) {
			return entity != Owner && (entity is Level || (entity is Door d && !d.Open) || entity.HasComponent<HealthComponent>() 
			                           || entity is SolidProp);
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent ev) {
				if (ev.Entity != Owner && ev.Entity.TryGetComponent<HealthComponent>(out var health)) {
					health.ModifyHealth(-Damage, Owner);
				}
				
				if (BreaksFrom(ev.Entity)) {
					Done = true;
				}
			}
			
			return base.HandleEvent(e);
		}

		public bool ShouldCollide(Entity entity) {
			return !(entity is Creature || entity is Chasm || entity is Item);
		}
	}
}