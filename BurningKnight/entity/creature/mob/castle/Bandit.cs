using System;
using BurningKnight.entity.component;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using Lens.entity;
using Lens.entity.component.logic;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.mob.castle {
	public class Bandit : Mob {
		protected override void SetStats() {
			base.SetStats();
			
			AddAnimation("bandit");
			SetMaxHp(1);
			
			Become<IdleState>();

			var body = new RectBodyComponent(2, 2, 12, 12);
			AddComponent(body);

			body.Body.LinearDamping = 10;
			moveId = Random.Int(0, 2);
		}

		private int moveId;
		
		#region Bandit States
		public class IdleState : CreatureState<Bandit> {
			private float delay;
			private float fireDelay;
			private bool fired;

			public override void Init() {
				base.Init();

				delay = Random.Float(1, 2.5f);
				fireDelay = Self.moveId % 2 == 0 ? 3 : Random.Float(0.5f, delay - 0.5f);
				Self.moveId++;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= delay) {
					Become<RunState>();
					return;
				}

				if (!fired && T >= fireDelay) {
					fired = true;
					
					if (Self.Target != null) {
						var ac = 0.1f;
						var angle = Self.AngleTo(Self.Target) + Random.Float(-ac, ac);
						var projectile = Projectile.Make(Self, "small", angle, 8f);

						projectile.AddLight(32f, Color.Red);
					}
				}
			}
		}
		
		public class RunState : CreatureState<Bandit> {
			private Vector2 velocity;
			private float timer;
			
			public override void Init() {
				base.Init();

				timer = Random.Float(0.4f, 1f);
				
				var angle = Random.AnglePI();
				var force = 120f + Random.Float(50f);
				
				if (Random.Chance() && Self.Target != null) {
					var ac = 0.1f;
					angle = Self.AngleTo(Self.Target) + Random.Float(-ac, ac);
				}
				
				velocity.X = (float) Math.Cos(angle) * force;
				velocity.Y = (float) Math.Sin(angle) * force;

				Self.GetComponent<RectBodyComponent>().Velocity = velocity;
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
			}

			public override void Update(float dt) {
				base.Update(dt);
				
				if (timer <= T) {
					Become<IdleState>();
				} else {
					Self.GetComponent<RectBodyComponent>().Velocity = velocity * Math.Min(1, timer - T * 0.4f);
				}
			}
		}
		#endregion

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent ev) {
				if (ev.Entity is Door) {
					var s = GetComponent<StateComponent>().StateInstance;

					if (s is RunState) {
						Become<IdleState>();
					}
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}