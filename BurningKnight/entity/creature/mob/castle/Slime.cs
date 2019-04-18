using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.level;
using Lens.entity;
using Lens.entity.component.logic;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.mob.castle {
	public class Slime : Mob {
		protected override void SetStats() {
			base.SetStats();
			
			AddComponent(new ZComponent());
			AddComponent(new ZAnimationComponent("slime"));
			SetMaxHp(2);

			if (Random.Chance()) {
				Become<JumpState>();
			} else {
				Become<IdleState>();
			}
			
			var body = new RectBodyComponent(2, 7, 12, 9);
			AddComponent(body);

			body.Body.LinearDamping = 1;
		}
		
		#region Slime States
		public class IdleState : MobState<Slime> {
			private float delay;

			public override void Init() {
				base.Init();
				
				delay = Random.Float(0.9f, 1.2f);
				Self.GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= delay) {
					Become<JumpState>();
				}
			}
		}

		public class JumpState : MobState<Slime> {
			private Vector2 velocity;
			private float zVelocity;
			
			public override void Init() {
				base.Init();

				var a = Self.Target == null ? Random.AnglePI() : Self.AngleTo(Self.Target);
				var force = Random.Float(20f) + 90f;
				
				velocity = new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
				Self.GetComponent<RectBodyComponent>().Velocity = velocity;
				
				zVelocity = 5;
			}

			public override void Update(float dt) {
				base.Update(dt);

				var component = Self.GetComponent<ZComponent>();
				component.Z += zVelocity * dt * 20;

				zVelocity -= dt * 10;
				
				if (component.Z <= 0) {
					component.Z = 0;
					Become<IdleState>();
				}
			}
		}
		#endregion

		public override bool InAir() {
			return (GetComponent<StateComponent>().StateInstance is JumpState);
		}

		public override bool ShouldCollide(Entity entity) {
			if (entity is Chasm) {
				return !InAir();
			}
			
			return base.ShouldCollide(entity);
		}

		public override bool HandleEvent(Event e) {
			if (e is HealthModifiedEvent ev && ev.Amount < 0 && InAir() && GetComponent<ZComponent>().Z > 4f) {
				return true;
			}
			
			return base.HandleEvent(e);
		}
	}
}