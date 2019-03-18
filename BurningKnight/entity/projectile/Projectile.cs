using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.level;
using Lens.entity;
using Lens.entity.component.graphics;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.projectile {
	public class Projectile : Entity {
		protected BodyComponent BodyComponent;
		public int Damage = 1;
		public Entity Owner;
		
		public Projectile(Entity owner, string slice, double angle, float speed, bool circle = true) {
			Owner = owner;
			
			var graphics = new SliceComponent("projectiles", slice);
			SetGraphicsComponent(graphics);

			var w = graphics.Sprite.Source.Width;
			var h = graphics.Sprite.Source.Height;

			if (circle) {
				AddComponent(BodyComponent = new CircleBodyComponent(0, 0, w / 2f, BodyType.Dynamic, false, true));
			} else {
				AddComponent(BodyComponent = new RectBodyComponent(0, 0, w, h, BodyType.Dynamic, false, true));
			}
			
			BodyComponent.Velocity = new Vector2((float) (Math.Cos(angle) * speed), (float) (Math.Sin(angle) * speed));
		}

		protected bool BreaksFrom(Entity entity) {
			return entity is Level;
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent ev) {
				if (ev.Entity.TryGetComponent<HealthComponent>(out var health)) {
					health.ModifyHealth(-Damage, Owner);
				}
				
				if (BreaksFrom(ev.Entity)) {
					Done = true;
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}