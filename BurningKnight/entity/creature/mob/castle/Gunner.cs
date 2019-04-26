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
	public class Gunner : Mob {
		protected override void SetStats() {
			base.SetStats();
			
			AddAnimation("gunner");
			SetMaxHp(5);
			
			Become<IdleState>();

			var body = new RectBodyComponent(2, 2, 12, 12);
			AddComponent(body);

			body.Body.LinearDamping = 6;
			moveId = Random.Int(3);
		}

		#region Gunner States
		public class IdleState : MobState<Gunner> {
			private float delay;

			public override void Init() {
				base.Init();
				delay = Random.Float(0.4f, 1f);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= delay) {
					Become<RunState>();
				}
			}
		}

		private int moveId;
		
		public class RunState : MobState<Gunner> {
			private const float Accuracy = 0.2f;
			
			private Vector2 velocity;
			private float timer;
			private float lastBullet;
			private float angle;
			private bool fire;
			private float start;
			private bool spread;
			
			public override void Init() {
				base.Init();

				spread = Random.Chance();
				fire = Self.Target != null && Self.moveId % 3 == 0;
				angle = !fire ? Random.AnglePI() : Self.AngleTo(Self.Target);
				timer = fire ? Random.Float(0.6f, 1.2f) : Random.Float(0.8f, 2f);
				start = Random.Float(0f, 10f);
				
				var a = angle + Random.Float(-Accuracy, Accuracy);
				var force = fire ? 60 : 120;
				
				velocity.X = (float) Math.Cos(a) * force;
				velocity.Y = (float) Math.Sin(a) * force;

				Self.GetComponent<RectBodyComponent>().Velocity = velocity;
				Self.moveId++;
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
			}

			public override void Update(float dt) {
				base.Update(dt);
				
				if (timer <= T) {
					Become<IdleState>();
					return;
				}

				var v = velocity * Math.Min(1, timer - T * 0.4f);
				Self.GetComponent<RectBodyComponent>().Velocity = v;

				if (!fire) {
					return;
				}

				lastBullet -= dt;
				
				if (lastBullet <= 0) {
					lastBullet = 0.05f;

					var a = angle + Random.Float(-Accuracy, Accuracy) + Math.Cos(T * 6f + start) * (float) Math.PI * (spread ? 0.3f : 0.07f);
					var projectile = Projectile.Make(Self, "small", a, spread ? 30f + 5f * T : (5f + 5f * T + v.Length()));

					projectile.AddLight(32f, Color.Red);
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

		public override bool CanSpawnMultiple() {
			return false;
		}

		public override float GetWeight() {
			return 2.5f;
		}
	}
}