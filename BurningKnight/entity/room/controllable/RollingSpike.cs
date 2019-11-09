using System;
using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.level;
using BurningKnight.level.entities;
using BurningKnight.physics;
using Lens.entity;
using Lens.util;
using Lens.util.file;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.room.controllable {
	public class RollingSpike : RoomControllable, CollisionFilterEntity {
		public Vector2 StartVelocity = new Vector2(32, 0);
		private List<Player> colliding = new List<Player>();
		
		public override void AddComponents() {
			base.AddComponents();

			var b = new CircleBodyComponent(0, 0, 8);
			AddComponent(b);
			
			b.Body.Restitution = 1;
			b.Body.Friction = 0;
			b.Body.Mass = 100000f;
			
			var a = new AnimationComponent("rolling_spike");
			AddComponent(a);

			AlwaysActive = true;
			a.OriginY *= 0.5f;
			
			AddComponent(new ShadowComponent());
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);

			var v = GetComponent<CircleBodyComponent>().Velocity;
			
			stream.WriteFloat(v.X);
			stream.WriteFloat(v.Y);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			StartVelocity = GetComponent<CircleBodyComponent>().Velocity = new Vector2(stream.ReadFloat(), stream.ReadFloat());
		}

		public override void PostInit() {
			base.PostInit();
			GetComponent<CircleBodyComponent>().Velocity = StartVelocity;
		}

		public override void Update(float dt) {
			base.Update(dt);

			foreach (var p in colliding) {
				if (p.InAir()) {
					continue;
				}
				
				p.GetComponent<HealthComponent>().ModifyHealth(-1, this);
				p.GetAnyComponent<BodyComponent>()?.KnockbackFrom(this, 4);
			}

			var velocity = GetComponent<CircleBodyComponent>().Velocity;
			GetComponent<AnimationComponent>().Angle += Math.Sign(Math.Abs(velocity.X) > 0.1f ? velocity.X : velocity.Y) * dt * 10;
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent ev) {
				var n = ev.Entity;

				if (!(n is Prop) && !(n is Level) && !(n is Chasm) && !(n is RollingSpike)) {
					if (n is Player p) {
						colliding.Add(p);
					}
				}
			} else if (e is CollisionEndedEvent cee && cee.Entity is Player p) {
				colliding.Remove(p);
			}
			
			return base.HandleEvent(e);
		}

		public bool ShouldCollide(Entity entity) {
			return !(entity is Projectile || (entity is Creature && !(entity is Player)));
		}
	}
}