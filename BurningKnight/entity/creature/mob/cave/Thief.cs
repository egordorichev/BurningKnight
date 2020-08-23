using System;
using BurningKnight.assets.lighting;
using BurningKnight.entity.component;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.cave {
	public class Thief : Mob {
		protected override void SetStats() {
			base.SetStats();

			Width = 16;
			Height = 16;
			
			AddAnimation("thief");
			SetMaxHp(4);
			
			Become<IdleState>();

			var body = new RectBodyComponent(2, 15, 12, 1);
			AddComponent(body);
			body.Body.LinearDamping = 10;

			AddComponent(new SensorBodyComponent(2, 2, 12, 14));
			AddComponent(new LightComponent(this, 20, new Color(0.5f, 1f, 0.4f)));
		}
		
		#region Bandit States
		public class IdleState : SmartState<Thief> {
			private float delay;

			public override void Init() {
				base.Init();
				delay = Rnd.Float(0.5f, 1f);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= delay) {
					Become<RunState>();
				}
			}
		}
		
		public class RunState : SmartState<Thief> {
			private Vector2 velocity;
			private float timer;
			
			public override void Init() {
				base.Init();

				timer = Rnd.Float(0.3f, 0.6f);
				
				var angle = Rnd.AnglePI();
				var force = 160f + Rnd.Float(90f);
				
				if (Rnd.Chance() && Self.Target != null) {
					var ac = 0.1f;
					angle = Self.AngleTo(Self.Target) + Rnd.Float(-ac, ac);
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

		protected override string GetDeadSfx() {
			return "mob_bandit_death";
		}

		protected override string GetHurtSfx() {
			return "mob_bandit_damage";
		}
	}
}